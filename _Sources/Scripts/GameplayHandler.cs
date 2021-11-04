using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayHandler : MonoBehaviour {

    [SerializeField]
    private float health;
    [SerializeField]
    private float maxHealth = 100.0f;
    [SerializeField]
    private Image healthBar;
    private float fillAmount;

    public static bool GameIsPaused = false;

    [SerializeField]
    private bool dead;
    [SerializeField]
    private bool paralyzed;
    [SerializeField]
    private GameObject respawnPoint;
    public float respawnTime = 10.0f;
    public float nextExplosion;

 
    
    /*=============================================================================*/

    void Awake() {

        dead = false;
        paralyzed = false;
        health = maxHealth;
        fillAmount = 1.0f;

    }

    void Update() {

        if(healthBar.fillAmount != fillAmount) {
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, fillAmount, Time.deltaTime * 10.0f);
        }

    }

    /*=============================================================================*/

    public void TakeDamage(float damage) {

        if(!dead) {

            health -= damage;
            health = Mathf.Clamp(health, 0.0f, maxHealth);
            fillAmount = health / maxHealth;

            if(health == 0.0f) { Die(); }

        }

    }

    public void Paralyze(float time) {
        GetComponent<Rigidbody>().freezeRotation = false;
        paralyzed = true;

        Invoke("UnParalyze", time);
    }
    private void UnParalyze() {
        GetComponent<Rigidbody>().freezeRotation = true;
        transform.position = new Vector3(transform.position.x, 1.1f, transform.position.z);
        paralyzed = false;
    }

    public void ExplosionDamage(Vector3 explosionPos, float maxDamage, float radius) {

        if(Time.time >= nextExplosion) {
            Vector3 forceVec = (transform.position - explosionPos).normalized;

            float distance = (transform.position - explosionPos).magnitude;
            Vector3 zeroDamageVec = forceVec * radius;
            Vector3 maxDamageVec = forceVec * (radius / 3.0f);
            
            if(distance <= radius / 3.0f) {
                TakeDamage(maxDamage);
            }
            else {

                if(distance <= (radius * (2.0f / 3.0f))) {
                    Paralyze(5.0f);
                }

                float damage = Scale(zeroDamageVec.magnitude, maxDamageVec.magnitude, 0.0f, maxDamage, distance);
                TakeDamage(damage);
            }

            nextExplosion = Time.time + 0.1f;
        }
        
    }

    void Die() {
        if(!dead) {

            dead = true;
            gameObject.GetComponent<Rigidbody>().freezeRotation = false;
            gameObject.GetComponent<PlayerController>().Die();

        } else {
            Debug.Log("WTF? He already DED!");
        }
    }

    public void Respawn() {

        health = maxHealth;
        fillAmount = 1.0f;
        dead = false;
        gameObject.GetComponent<Rigidbody>().freezeRotation = true;
        gameObject.transform.position = respawnPoint.transform.position;
        gameObject.transform.rotation = respawnPoint.transform.rotation;

    }

    public float Scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue) {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }

    /*=============================================================================*/

    public bool GetDead() { return dead; }
    public bool GetParalyzed() { return paralyzed; }

}
