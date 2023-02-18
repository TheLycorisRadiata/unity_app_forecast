using System;
using UnityEngine;

public class RotateHorizontally : MonoBehaviour
{
    [SerializeField] private UserInput input;

    private float rotateSpeed = 15f;
    public float pitch; // rotation around X

    void FixedUpdate()
    {
        pitch += -input.movementVector.x * rotateSpeed * Time.deltaTime;
        pitch = pitch < -360f || pitch > 360f ? 0f : pitch;
        transform.localRotation = Quaternion.Euler(0f, pitch, 0f);
    }
}
