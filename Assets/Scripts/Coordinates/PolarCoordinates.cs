using System;
using UnityEngine;

public class PolarCoordinates : MonoBehaviour
{
    private Transform earthModel;

    void Awake()
    {
        earthModel = transform.GetChild(0);
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
        return earthModel.InverseTransformPoint(raycastPoint);
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

    public Vector2 CalculateCoordinatesAtCenter()
    {
        Vector3 earthScreenPos = Camera.main.WorldToScreenPoint(transform.position);
        Ray ray = Camera.main.ScreenPointToRay(earthScreenPos);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
            return CalculateCoordinates(hit.point);
        else
            return new Vector2(float.NaN, float.NaN);
    }

    public Vector2 RoundUpPolarCoordinates(Vector2 coordinates, int digits)
    {
        coordinates.y = (float)Math.Round(coordinates.y, digits);
        coordinates.x = (float)Math.Round(coordinates.x, digits);
        return coordinates;
    }
}
