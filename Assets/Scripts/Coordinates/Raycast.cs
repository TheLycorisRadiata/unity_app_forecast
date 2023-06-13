using System;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    [SerializeField] private Transform _earth;
    [HideInInspector] public bool HasHit { get; private set; }
    [HideInInspector] public Vector3 Point { get; private set; }
    [HideInInspector] public Quaternion Rotation { get; private set; }

    void FixedUpdate()
    {
        PointerOnAnyCollider();
    }

    private void PointerOnAnyCollider()
    {
        Ray ray = Camera.main.ScreenPointToRay(UserInput.MousePosVector);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 90, Color.blue);
        
        HasHit = Physics.Raycast(ray, out hit);
        if (HasHit)
        {
            HasHit = true;
            Point = hit.point;
            Rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
    }

    public Tuple<Vector3, Quaternion> EarthCenter()
    {
        Vector3 earthScreenPos = Camera.main.WorldToScreenPoint(_earth.position);
        Ray ray = Camera.main.ScreenPointToRay(earthScreenPos);
        RaycastHit hit;
        Vector3 centerPoint;
        Quaternion centerRotation;

        Debug.DrawRay(ray.origin, ray.direction * 90, Color.red);

        centerPoint = new Vector3(float.NaN, float.NaN, float.NaN);
        centerRotation = Quaternion.identity;

        if (Physics.Raycast(ray, out hit))
        {
            centerPoint = hit.point;
            centerRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }

        return Tuple.Create(centerPoint, centerRotation);
    }
}
