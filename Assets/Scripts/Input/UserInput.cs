using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public static Vector2 movementVector, mousePosVector;
    public static float scroll;
    public static bool click, cancel;

    public void OnMove(InputAction.CallbackContext ctx)
    {
        movementVector = ctx.ReadValue<Vector2>();
    }

    public void OnMousePosition(InputAction.CallbackContext ctx)
    {
        mousePosVector = ctx.ReadValue<Vector2>();
    }

    public void OnScroll(InputAction.CallbackContext ctx)
    {
        /*
            0 (no scroll), 120 (scroll up), -120 (scroll down)
            --> There's no use for "120" so make it "1"
        */

        scroll = ctx.ReadValue<float>();
        scroll = Math.Clamp(scroll, -1f, 1f);
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        click = ctx.canceled ? false : true;
    }

    public void OnCancel(InputAction.CallbackContext ctx)
    {
        cancel = ctx.canceled ? false : true;
    }
}
