using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public class WebglRaycast : MonoBehaviour
{

    [SerializeField] Vector3 RaycastPoint = Vector3.zero;
    Camera cam;
   

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //draw ray
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Debug.DrawRay(ray.origin, ray.direction * 90, Color.blue);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            //Debug.Log(hit.point);
            RaycastPoint = hit.point;
    }
}
