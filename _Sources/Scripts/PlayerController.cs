using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 7.0f;
    [SerializeField]
    private float runningSpeed = 10.0f;
    [SerializeField]
    private float rotationSpeed = 0.2f;
    [SerializeField]
    private float jumpForce = 35.0f;
    [SerializeField]
    private float jumpCooldown = 2.0f;
    [SerializeField]
    private float knockoffForce = 1.0f;
    [SerializeField]
    private float distToGround;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip walkingSound;
    [SerializeField]
    private AudioClip runningSound;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject usedGun;
    [SerializeField]
    private GameObject gunPrefab;
    [SerializeField]
    private GameObject hatPrefab;
    [SerializeField]
    private GameObject usedHat;

    [Space]
    private Rigidbody rigidB;
    private Vector3 moveVector;
    private float nextJump;
    public bool isRunning;
    private bool isCrouching;
    private bool combatState;
    private bool knockedOff;
    private bool paralyzed;
    private GameplayHandler playerHandler;

    public Inventory inventory;
    public InputKeys keys;

    /*=============================================================================*/

    void Awake() {

        isRunning = true;
        isCrouching = false;
        combatState = false;
        knockedOff = false;
        paralyzed = false;

        inventory = new Inventory();
        keys = new InputKeys();

    }

    void Start() {

        rigidB = GetComponent<Rigidbody>();
        playerHandler = gameObject.GetComponent<GameplayHandler>();
        nextJump = Time.time + jumpCooldown;
        distToGround = GetComponent<Collider>().bounds.extents.y;

    }

    void FixedUpdate() {

        if(CanMove()) {

            LookMouseDirection();
            GetInput();

            Vector3 velocityVector = moveVector * moveSpeed;
            rigidB.velocity = new Vector3(velocityVector.x, rigidB.velocity.y, velocityVector.z);
        }

        if(playerHandler.GetDead() && !knockedOff) {
            knockedOff = true;
            rigidB.AddForce(moveVector * knockoffForce, ForceMode.Impulse);
        }

    }

    /*=============================================================================*/

    void GetInput() {

        float InputX = Input.GetAxis("Horizontal");
        float InputZ = Input.GetAxis("Vertical");

        moveVector = new Vector3(InputX, 0.0f, InputZ);
        moveVector = Vector3.ClampMagnitude(moveVector, 1.0f);

        if(Input.GetKeyDown(KeyCode.Space)) { Jump(); }

    }

    void LookMouseDirection() {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Utilities.GetMouseWorldPos(transform.gameObject, new Plane(Vector3.up, 0.0f)), Vector3.up), rotationSpeed);
    }
    
    void Jump() {

        if(IsGrounded() && Time.time >= nextJump) {
            nextJump = Time.time + jumpCooldown;
            rigidB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

    }

    public void Die() {
        
        usedGun.SetActive(false);
        usedHat.SetActive(false);
        Instantiate(gunPrefab, usedGun.transform.position, usedGun.transform.rotation);
        Instantiate(hatPrefab, usedHat.transform.position, usedHat.transform.rotation);

        Invoke("Respawn", playerHandler.respawnTime);

    }

    bool IsGrounded() {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    public bool CanMove() {

        if(!IsGrounded() || playerHandler.GetDead() || playerHandler.GetParalyzed() || knockedOff)
            return false;

        return true;
    }

    void Respawn() {
        rigidB.velocity = Vector3.zero;
        playerHandler.Respawn();
        usedGun.SetActive(true);
        usedHat.SetActive(true);
        usedGun.GetComponent<AutomaticGunScriptLPFP>().currentAmmo = usedGun.GetComponent<AutomaticGunScriptLPFP>().ammo;
    }

}
