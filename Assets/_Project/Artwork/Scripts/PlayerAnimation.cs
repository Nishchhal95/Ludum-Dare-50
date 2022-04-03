using HappyFlowGames.General;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public GeneralAudioOneShot SFX;
    // Animator and its variables
    public Animator PlayerAnimator;
    private string horizontal = "Horizontal";
    private string vertical = "Vertical";
    public enum PlayerState
    {
        Dead,
        Down,
        Idle,
        Left,
        Right,
        Up
    }

    private PlayerState currentState = PlayerState.Idle;

    [SerializeField] private PlayerInput playerInput;

    void Start()
    {
        AnimateTrigger(PlayerState.Idle);
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
                AnimateTrigger(PlayerState.Up);
                break;

            case KeyCode.F:
                AnimateTrigger(PlayerState.Down);
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
            AnimateTrigger(PlayerState.Right);
        }

        else if (playerInput.GetHorizontalInput() < 0)
        {
            AnimateTrigger(PlayerState.Left);
        }

        else
        {
            AnimateTrigger(PlayerState.Idle);
        }
    }

    /// <summary>
    /// Change animation state
    /// Animator does not check if transition is valid!
    /// </summary>
    /// <param name="state"></param>
    private void animate(PlayerState state)
    {

    }

    private void AnimateTrigger(PlayerState state)
    {
        if (currentState.Equals(state))
        {
            return;
        }
        Debug.Log("Playing State: " + state);
        currentState = state;
        // PlayerAnimator.SetTrigger(state);
        switch (state)
        {
            case PlayerState.Down:
                SFX.PlaySFX("Down");
                PlayerAnimator.SetFloat(horizontal, 0f);
                PlayerAnimator.SetFloat(vertical, -1f);
                break;
            default:
            case PlayerState.Idle:
                PlayerAnimator.SetFloat(horizontal, 0f);
                PlayerAnimator.SetFloat(vertical, 0f);
                break;
            case PlayerState.Left:
                PlayerAnimator.SetFloat(horizontal, -1f);
                PlayerAnimator.SetFloat(vertical, 0f);
                break;
            case PlayerState.Right:
                PlayerAnimator.SetFloat(horizontal, +1f);
                PlayerAnimator.SetFloat(vertical, 0f);
                break;
            case PlayerState.Up:
                SFX.PlaySFX("Up");
                PlayerAnimator.SetFloat(horizontal, 0f);
                PlayerAnimator.SetFloat(vertical, +1f);
                break;
        }
    }
}
