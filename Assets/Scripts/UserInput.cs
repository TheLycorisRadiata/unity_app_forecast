using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public Vector2 movementVector;
    public bool click;

    public void OnMove(InputAction.CallbackContext ctx)
    {
        movementVector = ctx.ReadValue<Vector2>();
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        click = ctx.canceled ? false : true;
    }
}
