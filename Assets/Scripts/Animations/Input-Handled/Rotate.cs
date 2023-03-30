using UnityEngine;

public class Rotate : MonoBehaviour
{
    private Transform cam;
    private float rotateSpeed = 20f;

    void Awake()
    {
        cam = Camera.main.transform;
    }

    void FixedUpdate()
    {
        if (MenuPointer.isPointerOnMenu)
            return;

        // horizontal --> rotation around X
        float pitch = UserInput.movementVector.x * rotateSpeed * Time.deltaTime;
        // vertical --> rotation around Y
        float yaw = UserInput.movementVector.y * rotateSpeed * Time.deltaTime;

        transform.Rotate(-Vector3.up, pitch, Space.World);
        transform.Rotate(cam.right, yaw, Space.World);
    }
}
