using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public static Vector2 MovementVector { get; private set; }
    public static Vector2 MousePosVector { get; private set; }
    public static float Scroll { get; private set; }
    public static bool Click { get; private set; }
    public static bool Cancel { get; private set; }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        MovementVector = ctx.ReadValue<Vector2>();
    }

    public void OnMousePosition(InputAction.CallbackContext ctx)
    {
        MousePosVector = ctx.ReadValue<Vector2>();
    }

    public void OnScroll(InputAction.CallbackContext ctx)
    {
        /*
            0 (no scroll), 120 (scroll up), -120 (scroll down)
            --> There's no use for "120" so make it "1"
        */

        Scroll = ctx.ReadValue<float>();
        Scroll = Math.Clamp(Scroll, -1f, 1f);
    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        Click = ctx.canceled ? false : true;
    }

    public void OnCancel(InputAction.CallbackContext ctx)
    {
        Cancel = ctx.canceled ? false : true;
    }
}
