using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {

    public GameObject target;
    public Vector3 cameraOffset = new Vector3(0.0f, 10.0f, 0.0f);
    public float smoothSpeed = 10.0f;
    public bool lookAtTarget = true;

    /*=============================================================================*/

    void LateUpdate() {

        Vector3 desiredPos = target.transform.position + cameraOffset;

        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPos;

        if(lookAtTarget) {
            transform.LookAt(target.transform, Vector3.forward);
        }

    }

    /*=============================================================================*/


}
