using System;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    [SerializeField] private Transform earth;
    public bool hasHit;
    public Vector3 point;
    public Quaternion rotation;

    void FixedUpdate()
    {
        PointerOnAnyCollider();
    }

    private void PointerOnAnyCollider()
    {
        Ray ray = Camera.main.ScreenPointToRay(UserInput.mousePosVector);
        Debug.DrawRay(ray.origin, ray.direction * 90, Color.blue);

        RaycastHit hit;
        hasHit = Physics.Raycast(ray, out hit);
        if (hasHit)
        {
            hasHit = true;
            point = hit.point;
            rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
    }

    public Tuple<Vector3, Quaternion> EarthCenter()
    {
        Vector3 earthScreenPos = Camera.main.WorldToScreenPoint(earth.position);
        Ray ray = Camera.main.ScreenPointToRay(earthScreenPos);
        Debug.DrawRay(ray.origin, ray.direction * 90, Color.red);

        RaycastHit hit;
        Vector3 centerPoint = new Vector3(float.NaN, float.NaN, float.NaN);
        Quaternion centerRotation = Quaternion.identity;

        if (Physics.Raycast(ray, out hit))
        {
            centerPoint = hit.point;
            centerRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }

        return Tuple.Create(centerPoint, centerRotation);
    }
}
