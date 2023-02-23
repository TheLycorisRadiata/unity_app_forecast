using UnityEngine;

public class PolarCoordinates : MonoBehaviour
{
    public float latitude, longitude;
    [SerializeField] private CartesianCoordinates cartesianScript;

    void Update()
    {
        // The cartesian X and Y coordinates are a -360/+360 range with (0,0) being Null Island
        // The Z coordinate is used to represent the sphere curvature
        Vector3 cartesian = cartesianScript.coordinates;
        Vector3 dir = Vector3.zero;

        // Translate the -360/+360 range into latitude and longitude

        // * If you go east (result: 0 to 180, -180 to 0)
        if (cartesian.x > 180f && cartesian.x <= 360f)
            cartesian.x = cartesian.x - 360f;
        // * If you go west (result: 0 to -180, 180 to 0)
        else if (cartesian.x <= -180f && cartesian.x >= -360f)
            cartesian.x = cartesian.x + 360f;

        // * If you go north (result: 0 to 90, 90 to 0, 0 to -90, -90 to 0)
        if (cartesian.y > 90f && cartesian.y <= 270f)
            cartesian.y = (cartesian.y - 180f) * -1f;
        else if (cartesian.y > 270f && cartesian.y <= 360f)
            cartesian.y = cartesian.y - 360f;

        // * If you go south (result: 0 to -90, -90 to 0, 0 to 90, 90 to 0)
        if (cartesian.y < -90f && cartesian.y >= -270f)
            cartesian.y = cartesian.y * -1f - 180f;
        else if (cartesian.y < -270f && cartesian.y >= -360f)
            cartesian.y = cartesian.y + 360f;

        // Apply the curvature
        dir = cartesian.normalized;
        latitude = Mathf.Asin(dir.y) * Mathf.Rad2Deg;
        longitude = Mathf.Atan2(dir.x, - dir.z) * Mathf.Rad2Deg;

        // CHECK: Real latitude and longitude --> Nice (43.7, 7.2) and London (51.5, 0)
    }
}
