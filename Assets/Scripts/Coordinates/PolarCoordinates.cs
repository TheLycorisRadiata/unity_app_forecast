using UnityEngine;

public class PolarCoordinates : MonoBehaviour
{
    private Transform _earthModel;

    void Awake()
    {
        _earthModel = transform.GetChild(0);
    }

    /* y is latitude and x is longitude */
    public Vector2 CalculateCoordinates(Vector3 raycastPoint)
    {
        /*
        Vector3 coordinates = earthModel.InverseTransformPoint(raycastPoint);
        coordinates.y = 90f - Mathf.Acos(coordinates.y / coordinates.magnitude) * Mathf.Rad2Deg;
        coordinates.x = Mathf.Atan2(coordinates.z, coordinates.x) * Mathf.Rad2Deg;
        return new Vector2(coordinates.x, coordinates.y);
        */

        Vector3 cartesianCoordinates = CalculateCartesianCoordinates(raycastPoint);
        Vector3 sphericalCoordinates = CalculateSphericalCoordinates(cartesianCoordinates);
        return CalculatePolarCoordinates(sphericalCoordinates);
    }

    private Vector3 CalculateCartesianCoordinates(Vector3 raycastPoint)
    {
        return _earthModel.InverseTransformPoint(raycastPoint);
    }

    private Vector3 CalculateSphericalCoordinates(Vector3 cartesianCoordinates)
    {
        float r = cartesianCoordinates.magnitude;
        float theta = Mathf.Acos(cartesianCoordinates.y / r);
        float phi = Mathf.Atan2(cartesianCoordinates.z, cartesianCoordinates.x);
        return new Vector3(r, theta, phi) * Mathf.Rad2Deg;
    }

    private Vector2 CalculatePolarCoordinates(Vector3 sphericalCoordinates)
    {
        float longitude = sphericalCoordinates.z;
        float latitude = 90f - sphericalCoordinates.y;
        return new Vector2(longitude, latitude);
    }
}
