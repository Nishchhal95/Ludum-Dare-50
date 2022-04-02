using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float jumpForce = 0;
    
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float midToGroundOffset;

    [SerializeField] private bool jumpKeyPressed = false;
    
    [SerializeField] private bool useRawInput = false;

    [SerializeField] private float horizontalInput = 0;
    
    private void Update()
    {
        HandleInput();
        HandleGrounded();
    }

    private void FixedUpdate()
    {
        HandlePlayerPhysics();
    }

    private void HandleInput()
    {
        horizontalInput = useRawInput ? Input.GetAxisRaw("Horizontal") : Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpKeyPressed = true;
            jumpForce = Mathf.Sqrt(-2 * (Physics2D.gravity.y * playerRb.gravityScale) * jumpHeight);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            FallDown();
        }
    }

    private void FallDown()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("Ground"),
            true);
        DoActionAfterXSeconds(() =>
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("Ground"),
                false); 
        }, .2f);
    }

    private void HandleGrounded()
    {
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, -transform.up, 
            (transform.localScale.y / 2) + midToGroundOffset, groundLayer);
        if (hit2D.collider == null)
        {
            isGrounded = false;
            return;
        }

        isGrounded = true;
    }
    
    private void HandlePlayerPhysics()
    {
        float yVelocity = jumpKeyPressed && isGrounded ? jumpForce : playerRb.velocity.y;
        jumpKeyPressed = false;
        playerRb.velocity = new Vector2(horizontalInput * playerSpeed, yVelocity);
    }

    private void DoActionAfterXSeconds(Action action, float seconds)
    {
        StartCoroutine(DoActionAfterXSecondsRoutine(action, seconds));
    }

    private IEnumerator DoActionAfterXSecondsRoutine(Action action, float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        action?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, -transform.up * ((transform.localScale.y / 2) + midToGroundOffset));
    }
}
