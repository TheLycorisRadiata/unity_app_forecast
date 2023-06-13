using UnityEngine;

public class Rotate : MonoBehaviour
{
    private Transform _cam;
    private float _rotateSpeed = 20f;

    void Awake()
    {
        _cam = Camera.main.transform;
    }

    void FixedUpdate()
    {
        if (MenuPointer.IsPointerOnMenu)
            return;

        transform.Rotate(-Vector3.up, GetPitch(), Space.World);
        transform.Rotate(_cam.right, GetYaw(), Space.World);
    }

    private float GetPitch()
    {
        // horizontal --> rotation around X
        return UserInput.MovementVector.x * _rotateSpeed * Time.deltaTime;
    }

    private float GetYaw()
    {
        // vertical --> rotation around Y
        return UserInput.MovementVector.y * _rotateSpeed * Time.deltaTime;
    }
}
