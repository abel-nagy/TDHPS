using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour {

    public float afterXseconds = 10.0f;



    // Start is called before the first frame update
    void Start() {
        Invoke("DestroySelf", afterXseconds);
    }
    void DestroySelf() {
        Destroy(gameObject);
    }

}
