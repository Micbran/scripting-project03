using System;
using UnityEngine;

public class GrappleInput : MonoBehaviour
{
    public event Action GrapplePressed = delegate { };
    public event Action GrappleReleased = delegate { };


    private void Update()
    {
        DetectGrappleInput();
    }

    private void DetectGrappleInput()
    {
        if(Input.GetMouseButtonDown((int)MouseButton.RightClick))
        {
            GrapplePressed.Invoke();
        }
        else if(Input.GetMouseButtonUp((int)MouseButton.RightClick))
        {
            GrappleReleased.Invoke();
        }
    }
}

public enum MouseButton
{
    LeftClick = 0,
    RightClick = 1,
    MiddleClick = 2
}
