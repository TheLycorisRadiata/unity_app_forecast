using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebglRaycast : MonoBehaviour
{
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam= Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //draw ray
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        mousePos= cam.ScreenToViewportPoint(mousePos);
        Debug.DrawRay(transform.position, mousePos,Color.blue);
    }
}
