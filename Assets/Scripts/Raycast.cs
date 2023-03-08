using UnityEngine;
using UnityEngine.InputSystem;

public class Raycast : MonoBehaviour
{
    public Vector3 point;
    public Quaternion rotation;
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Debug.DrawRay(ray.origin, ray.direction * 90, Color.blue);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            point = hit.point;
            rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
    }
}
