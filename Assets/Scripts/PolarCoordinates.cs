using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolarCoordinates : MonoBehaviour
{
    public float latitude, longitude;
    [SerializeField] private CartesianCoordinates cartesianScript;

    void Update()
    {
        Vector3 dir = cartesianScript.coordinates.normalized;
        latitude = Mathf.Asin(dir.y) * Mathf.Rad2Deg;
        longitude = Mathf.Atan2(dir.x, - dir.z) * Mathf.Rad2Deg;

        // CHECK: Real latitude and longitude --> Nice (43.7, 7.2) and London (51.5, 0)
    }
}
