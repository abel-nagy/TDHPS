using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingOutOfWorld : MonoBehaviour {

    [SerializeField]
    private float bounds = -20.0f;
    [SerializeField]
    private float damageAmount = 10.0f;
    [SerializeField]
    private float damageInterval = 1.0f;
    [SerializeField]
    private float nextDamage;

    /*=============================================================================*/

    void Start() {
        nextDamage = Time.time + damageInterval;
    }

    // Update is called once per frame
    void Update() {

        if(gameObject.transform.position.y <= bounds && Time.time >= nextDamage) {
            nextDamage = Time.time + damageInterval;
            gameObject.GetComponent<GameplayHandler>().TakeDamage(damageAmount);
        }

    }
}
