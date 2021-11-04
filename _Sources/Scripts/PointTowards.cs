using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTowards : MonoBehaviour {

    PlayerController pc = new PlayerController();

    public float rotationSpeed = 1;
    public float minDist = 1.0f;

    public GameObject toRotate;


    /*=============================================================================*/


    void Update() {

        if(!GameplayHandler.GameIsPaused)
            PointToward();

    }


    /*=============================================================================*/


    void PointToward() {
        Vector3 mousePos = Utilities.GetMouseWorldPos(toRotate, new Plane(Vector3.up, 0.0f));
        float distance = (toRotate.transform.position - mousePos).magnitude;

        if(distance >= minDist) {
            toRotate.transform.rotation = Quaternion.Slerp(toRotate.transform.rotation, Quaternion.LookRotation(mousePos, Vector3.up), rotationSpeed);
            Vector3 rotation = toRotate.transform.rotation.eulerAngles;
            
            rotation.y -= Mathf.Lerp(0.0f, 3.0f, Mathf.InverseLerp(2.0f, 15.0f, distance));
            toRotate.transform.rotation = Quaternion.Euler(rotation);
        }

    }

}
