using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private UserInput input;
    private Transform cam;
    private float rotateSpeed = 20f;

    void Awake()
    {
        cam = Camera.main.transform;
    }

    void FixedUpdate()
    {
        // horizontal --> rotation around X
        float pitch = input.movementVector.x * rotateSpeed * Time.deltaTime;
        // vertical --> rotation around Y
        float yaw = input.movementVector.y * rotateSpeed * Time.deltaTime;

        transform.Rotate(-Vector3.up, pitch, Space.World);
        transform.Rotate(cam.right, yaw, Space.World);
    }
}
