using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class WebglRaycast : MonoBehaviour
{
    public Vector3 RaycastPoint;
    public Quaternion RaycastRotation;
    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        //draw ray
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Debug.DrawRay(ray.origin, ray.direction * 90, Color.blue);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //Debug.Log(hit.point);
            RaycastPoint = hit.point;
            RaycastRotation =  Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
    }
}
