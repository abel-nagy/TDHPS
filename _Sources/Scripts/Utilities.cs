using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities {

    public static Vector3 GetMouseWorldPos(GameObject gameObject, Plane plane) {

        float distance;
        Vector3 mousePos = new Vector3(0, 0, 0);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(plane.Raycast(ray, out distance)) {
            mousePos = ray.GetPoint(distance);
            mousePos = mousePos - gameObject.transform.position;
            mousePos.y = 0.0f;
        }
        
        return mousePos;

    }

}
