using UnityEngine;

public class RotateHorizontally : MonoBehaviour
{
    [SerializeField] private UserInput input;

    private float rotateSpeed = 15f;
    private float pitch; // rotation around X

    void FixedUpdate()
    {
        pitch += -input.movementVector.x * rotateSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0f, pitch, 0f);
    }
}
