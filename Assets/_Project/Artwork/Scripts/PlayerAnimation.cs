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

    void Start()
    {
        animate(idle);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
            animate(dead);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            animate(down);
        if (Input.GetKeyDown(KeyCode.Space))
            animate(idle);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            animate(left);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            animate(right);
        if (Input.GetKeyDown(KeyCode.UpArrow))
            animate(up);
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
}
