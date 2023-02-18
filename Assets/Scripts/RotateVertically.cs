using UnityEngine;

public class RotateVertically : MonoBehaviour
{
    [SerializeField] private UserInput input;

    private float rotateSpeed = 15f;
    public float yaw; // rotation around Y

    void FixedUpdate()
    {
        yaw += input.movementVector.y * rotateSpeed * Time.deltaTime;
        yaw = yaw < -360f || yaw > 360f ? 0f : yaw;
        transform.localRotation = Quaternion.Euler(yaw, 0f, 0f);
    }
}
