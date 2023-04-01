using System;
using UnityEngine;

public class PolarCoordinates : MonoBehaviour
{
    private Transform earthModel;

    void Awake()
    {
        earthModel = transform.GetChild(0);
    }

    public Vector2 CalculateCoordinates(Vector3 raycastPoint)
    {
        /* y is latitude and x is longitude */
        Vector3 coordinates = earthModel.InverseTransformPoint(raycastPoint);
        coordinates.y = 90f - Mathf.Acos(coordinates.y / coordinates.magnitude) * Mathf.Rad2Deg;
        coordinates.x = Mathf.Atan2(coordinates.z, coordinates.x) * Mathf.Rad2Deg;

        /* Limit to 2 decimals after the floating point */
        coordinates.y = (float)Math.Round(coordinates.y, 2);
        coordinates.x = (float)Math.Round(coordinates.x, 2);

        return new Vector2(coordinates.x, coordinates.y);
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
}
