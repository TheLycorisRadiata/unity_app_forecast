using System;
using UnityEngine;

public class RotateEarth : MonoBehaviour
{
    [SerializeField] private UserInput input;
    [SerializeField] private float rotateSpeed = 15f;

    void FixedUpdate()
    {
        float horizontalInput = -input.movementVector.x * rotateSpeed * Time.deltaTime;
        float verticalInput = input.movementVector.y * rotateSpeed * Time.deltaTime;

        bool horIsStronger = Math.Abs(horizontalInput) > Math.Abs(verticalInput);
        Vector3 newEulerAngles = horIsStronger ? new Vector3(0f, horizontalInput, 0f) : new Vector3(verticalInput, 0f, 0f);
        
        transform.Rotate(newEulerAngles, Space.World);
    }
}
