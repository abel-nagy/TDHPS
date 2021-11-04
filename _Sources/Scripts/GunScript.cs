using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;



public class GunScript : MonoBehaviour {


    public Animator anim;
    public GameObject player;
    private PlayerController pc;
    private Inventory inventory;
    private InputKeys keys;

    [Space]

    // Weapon
    [Header("Weapon Settings")]
    public string gunName;
    public Guns gunId;
    private float lastFired;
    [Tooltip("How fast the weapon fires, higher value means faster rate of fire.")]
    public float fireRate;
    [Tooltip("Enables auto reloading when out of ammo.")]
    public bool autoReload;
    public float autoReloadDelay;
    public float magSpawnDelay;
    public float reloadTime = 3.4f;

    // Actions
    private bool isReloading;
    private bool isAiming;
    private bool hasBeenHolstered = false;
    private bool holstered;

    [Space]

    // Ammo
    public int currentAmmo;
    [Tooltip("How much ammo the weapon magazine can hold.")]
    public int magAmmo;
    private bool outOfAmmo;

    // Bullet
    [Header("Bullet")]
    [Tooltip("How much force is applied to the bullet when shooting.")]
    public float bulletForce = 400.0f;

    [Space]

    // Muzzle flash
    [Header("Muzzleflash Settings")]
    public bool randomMuzzleFlash;
    private int minRandomValue = 1;
    [Range(2, 25)]
    public int maxRandomValue = 5;
    private int randomMuzzleflashValue;
    public bool enableMuzzleflash = true;
    public ParticleSystem muzzleParticles;
    public bool enableSparks = true;
    public ParticleSystem sparkParticles;
    public int minSparkEmission = 1;
    public int maxSparkEmission = 7;

    // Muzzleflash light
    [Header("Muzzleflash Light Settings")]
    public Light muzzleflashLight;
    public float lightDuration = 0.02f;

    [Space]

    // Audio sources
    [Header("Audio sources")]
    public AudioSource mainAudioSource;
    public AudioSource shootAudioSource;

    // UI
    [Header("UI Components")]
    public Text currentWeaponText;
    public Text ammoText;


    // Transforms
    [System.Serializable]
    public class Prefabs {
        [Header("Prefabs")]
        public Transform bulletPrefab;
        public Transform casingPrefab;
        public Transform magPrefab;
    }
    public Prefabs prefabs;

    [System.Serializable]
    public class Spawnpoints {
        [Header("Spawnpoints")]
        public Transform casingSpawnPoint;
        public Transform bulletSpawnPoint;
        public Transform magSpawnPoint;
    }
    public Spawnpoints spawnpoints;

    [System.Serializable]
    public class SoundClips {
        public AudioClip shootSound;
        public AudioClip takeOutSound;
        public AudioClip holsterSound;
        public AudioClip reloadSoundOutOfAmmo;
        public AudioClip reloadSoundAmmoLeft;
        public AudioClip aimSound;
    }
    public SoundClips soundClips;
    private bool soundHasPlayer = false;


    /*=============================================================================*/


    void Awake() {

        anim = GetComponent<Animator>();
        muzzleflashLight.enabled = false;

    }


    void Start() {

        pc = player.GetComponent<PlayerController>();
        inventory = pc.inventory;
        keys = pc.keys;

        currentWeaponText.text = gunName;
        ammoText.text = currentAmmo.ToString() + " / " + inventory.gunAmmo[(int)gunId].ToString();

        shootAudioSource.clip = soundClips.shootSound;

    }


    void Update() {

        // Aiming

        // Aiming END

        ammoText.text = currentAmmo.ToString() + " / " + inventory.gunAmmo[(int)gunId].ToString();

        if(currentAmmo == 0) {
            outOfAmmo = true;

            if(autoReload && !isReloading) {
                StartCoroutine(AutoReload());
            }

        }

        AnimCheck();
        GetInput();

    }


    /*=============================================================================*/


    private void GetInput() {


        // Shoot
        if(Input.GetMouseButton(0) && !outOfAmmo && !isReloading && pc.CanMove()) {

            if(Time.time - lastFired > 1 / fireRate) {

                lastFired = Time.time;

                currentAmmo -= 1;

                shootAudioSource.clip = soundClips.shootSound;
                shootAudioSource.Play();

                if(!isAiming) {
                    anim.Play("Fire", 0, 0f);
                } else {
                    anim.Play("Aim Fire", 0, 0f);
                }

                DoMuzzleFlash();
                Shoot();

            }

        }

        // Holster Weapon
        if(Input.GetKeyDown(keys.weaponHolster) ) {

            if(!hasBeenHolstered) {

                holstered = false;
                mainAudioSource.clip = soundClips.holsterSound;
                mainAudioSource.Play();
                hasBeenHolstered = true;
                anim.SetBool("Holster", false);

            } else {

                holstered = true;
                mainAudioSource.clip = soundClips.takeOutSound;
                mainAudioSource.Play();
                hasBeenHolstered = false;
                anim.SetBool("Holster", true);

            }

        }

        // Reload
        if(Input.GetKeyDown(keys.reloadWeapon) && !isReloading && !holstered && currentAmmo != magAmmo && inventory.gunAmmo[(int)gunId] > 0)
            Reload();


    }


    private void Reload() {

        int reloadedAmmo = 0;

        if(outOfAmmo) {

            StartCoroutine(ReloadCountDown());

            anim.Play("Reload Out Of Ammo", 0, 0f);
            mainAudioSource.clip = soundClips.reloadSoundOutOfAmmo;
            mainAudioSource.Play();

            StartCoroutine(MagSpawnDelay());

            if(inventory.gunAmmo[(int)gunId] >= magAmmo) {

                reloadedAmmo = magAmmo;
                inventory.gunAmmo[(int)gunId] -= magAmmo;

            } else {

                reloadedAmmo = inventory.gunAmmo[(int)gunId];
                inventory.gunAmmo[(int)gunId] -= reloadedAmmo;

            }

        } else {

            StartCoroutine(ReloadCountDown());

            anim.Play("Reload Ammo Left", 0, 0f);
            mainAudioSource.clip = soundClips.reloadSoundAmmoLeft;
            mainAudioSource.Play();

            StartCoroutine(MagSpawnDelay());

            if(inventory.gunAmmo[(int)gunId] >= (magAmmo - currentAmmo)) {

                reloadedAmmo = (magAmmo - currentAmmo);
                inventory.gunAmmo[(int)gunId] -= reloadedAmmo;

            } else {

                reloadedAmmo = inventory.gunAmmo[(int)gunId];
                inventory.gunAmmo[(int)gunId] -= reloadedAmmo;

            }

        }

        currentAmmo += reloadedAmmo;
        outOfAmmo = false;


    }


    /*=============================================================================*/


    private IEnumerator MagSpawnDelay() {

        yield return new WaitForSeconds(magSpawnDelay);

        Instantiate(prefabs.magPrefab, spawnpoints.magSpawnPoint.transform.position + transform.position, transform.rotation);

    }


    private IEnumerator AutoReload() {

        yield return new WaitForSeconds(autoReloadDelay);

        if(inventory.gunAmmo[(int)gunId] > 0)
            Reload();

    }


    private IEnumerator MuzzleFlashLight() {

        muzzleflashLight.enabled = true;
        yield return new WaitForSeconds(lightDuration);
        muzzleflashLight.enabled = true;

    }

    private IEnumerator ReloadCountDown() {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        isReloading = false;
    }


    /*=============================================================================*/


    private void Shoot() {

        var bullet = (Transform)Instantiate(prefabs.bulletPrefab, spawnpoints.bulletSpawnPoint.transform.position, spawnpoints.bulletSpawnPoint.transform.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletForce;
        Instantiate(prefabs.casingPrefab, spawnpoints.casingSpawnPoint.transform.position, spawnpoints.casingSpawnPoint.transform.rotation);

    }


    private void DoMuzzleFlash() {

        if(!randomMuzzleFlash && enableMuzzleflash == true) {
            muzzleParticles.Emit(1);
            StartCoroutine(MuzzleFlashLight());
        } else {
            if(randomMuzzleflashValue == 1) {
                if(enableSparks)
                    sparkParticles.Emit(Random.Range(minSparkEmission, maxSparkEmission));
                if(enableMuzzleflash) {
                    muzzleParticles.Emit(1);
                    StartCoroutine(MuzzleFlashLight());
                }
            }
        }

    }

    private void AnimCheck() {
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Reload Out Of Ammo") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Reload Ammo left")) {

            isReloading = false;
            
        }
    }


}
