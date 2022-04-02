using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private bool useRawInput = false;
    [SerializeField] private float horizontalInput = 0;

    public Action<KeyCode> onKeyPressed;
    
    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        horizontalInput = useRawInput ? Input.GetAxisRaw("Horizontal") : Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            onKeyPressed?.Invoke(KeyCode.Space);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            onKeyPressed?.Invoke(KeyCode.F);
        }
    }

    public float GetHorizontalInput()
    {
        return horizontalInput;
    }
}
