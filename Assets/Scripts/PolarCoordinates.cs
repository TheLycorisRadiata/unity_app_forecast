using UnityEngine;

public class PolarCoordinates : MonoBehaviour
{
    [SerializeField] private WebglRaycast raycastScript;
    public Vector3 coordinates;

    void Update()
    {
        coordinates = transform.InverseTransformPoint(raycastScript.RaycastPoint);
        coordinates.y = 90f - Mathf.Acos(coordinates.y / coordinates.magnitude) * Mathf.Rad2Deg;
        coordinates.x = Mathf.Atan2(coordinates.z, coordinates.x) * Mathf.Rad2Deg;
    }
}
