using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{

    // Animator and its variables
    public Animator PlayerAnimator;
    private string dead = "Dead";
    private string down = "Down";
    private string idle = "Idle";
    private string left = "Left";
    private string right = "Right";
    private string up = "Up";

    private string currentState = "Idle";

    [SerializeField] private PlayerInput playerInput;

    void Start()
    {
        AnimateTrigger(idle);
    }

    private void OnEnable()
    {
        playerInput.onKeyPressed += OnKeyPressed;
    }

    private void OnDisable()
    {
        playerInput.onKeyPressed -= OnKeyPressed;
    }
    
    private void OnKeyPressed(KeyCode obj)
    {
        switch (obj)
        {
            case KeyCode.Space:
                AnimateTrigger(up);
                break;
            
            case KeyCode.F:
                AnimateTrigger(down);
                break;
        }
    }

    void Update()
    {
        ProcessPlayerInput();
        
        // if (Input.GetKeyDown(KeyCode.X))
        //     animate(dead);
        // if (Input.GetKeyDown(KeyCode.DownArrow))
        //     animate(down);
        // if (Input.GetKeyDown(KeyCode.Space))
        //     animate(idle);
        // if (Input.GetKeyDown(KeyCode.LeftArrow))
        //     animate(left);
        // if (Input.GetKeyDown(KeyCode.RightArrow))
        //     animate(right);
        // if (Input.GetKeyDown(KeyCode.UpArrow))
        //     animate(up);
    }

    private void ProcessPlayerInput()
    {
        if (playerInput.GetHorizontalInput() > 0)
        {
            AnimateTrigger(right);
        }

        else if(playerInput.GetHorizontalInput() < 0)
        {
            AnimateTrigger(left);
        }

        else
        {
            AnimateTrigger(idle);
        }
    }

    /// <summary>
    /// Change animation state
    /// Animator does not check if transition is valid!
    /// </summary>
    /// <param name="state"></param>
    private void animate (string state)
    {
        PlayerAnimator.SetBool(dead, false);
        PlayerAnimator.SetBool(down, false);
        PlayerAnimator.SetBool(idle, false);
        PlayerAnimator.SetBool(left, false);
        PlayerAnimator.SetBool(right, false);
        PlayerAnimator.SetBool(up, false);

        PlayerAnimator.SetBool(state, true);
    }

    private void AnimateTrigger(string state)
    {
        if (currentState.Equals(state))
        {
            return;
        }
        Debug.Log("Playing State: " + state);
        currentState = state;
        PlayerAnimator.SetTrigger(state);
    }
}
