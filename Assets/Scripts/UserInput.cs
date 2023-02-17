using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public Vector2 movementVector;
    public float scroll;
    public bool click;

    public void OnMove(InputAction.CallbackContext ctx)
    {
        movementVector = ctx.ReadValue<Vector2>();
    }

    public void OnScroll(InputAction.CallbackContext ctx)
    {
        /*
            0 (no scroll), 120 (scroll up), -120 (scroll down)
            --> If positive then there has been a scroll up, if negative it's a scroll down
            --> There's no use for "120", so divide the value by 120 in order to get either 1 or -1
        */

        scroll = ctx.ReadValue<float>();
        if (scroll != 0f)
            scroll /= 120f;
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        click = ctx.canceled ? false : true;
    }
}
