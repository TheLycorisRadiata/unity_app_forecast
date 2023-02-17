using UnityEngine;

public class RotateVertically : MonoBehaviour
{
    [SerializeField] private UserInput input;

    private float rotateSpeed = 15f;
    private float yaw; // rotation around Y

    void FixedUpdate()
    {
        yaw += input.movementVector.y * rotateSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(yaw, 0f, 0f);
    }
}
