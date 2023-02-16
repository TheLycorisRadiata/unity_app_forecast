using UnityEngine;

public class RotateEarth : MonoBehaviour
{
    [SerializeField] private UserInput input;

    private float rotateSpeed = 15f;
    private float yaw; // rotation around Y
    private float pitch; // rotation around X

    void FixedUpdate()
    {
        yaw += input.movementVector.y * rotateSpeed * Time.deltaTime;
        pitch += -input.movementVector.x * rotateSpeed * Time.deltaTime;

        transform.localRotation = Quaternion.Euler(yaw, pitch, 0f);
    }
}
