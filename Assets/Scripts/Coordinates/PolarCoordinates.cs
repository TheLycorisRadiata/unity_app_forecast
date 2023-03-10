using System;
using UnityEngine;

public class PolarCoordinates : MonoBehaviour
{
    [SerializeField] private LocationScriptableObject location;
    private Transform earthModel;

    void Awake()
    {
        earthModel = transform.GetChild(0);
    }

    private Vector2 CalculateCoordinates(Vector3 raycastPoint)
    {
        Vector3 coordinates = earthModel.InverseTransformPoint(raycastPoint);
        coordinates.y = 90f - Mathf.Acos(coordinates.y / coordinates.magnitude) * Mathf.Rad2Deg;
        coordinates.x = Mathf.Atan2(coordinates.z, coordinates.x) * Mathf.Rad2Deg;
        return new Vector2(coordinates.x, coordinates.y);
    }

    public void UpdateLocationCoordinates(Vector3 raycastPoint)
    {
        Vector2 coordinates = CalculateCoordinates(raycastPoint);
        location.latitude = (float)Math.Round(coordinates.y, 2);
        location.longitude = (float)Math.Round(coordinates.x, 2);
    }
}
