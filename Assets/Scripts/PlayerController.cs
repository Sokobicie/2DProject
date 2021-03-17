using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float horizontalInput = 0f;
    public float movementSpeed = 3f;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private bool isGrounded = false;
    private bool isJumping = false;
    public float jumpForce = 5f;
    private Collider2D coll;
    private ContactFilter2D contactFilter;
    public LayerMask layerMask;
    private RaycastHit2D[] results = new RaycastHit2D[10];
    public Rigidbody2D rb;
    
    void Start()
    {
        coll = GetComponent<Collider2D>();
        contactFilter.SetLayerMask(layerMask);        
    }

    void Update()
    {
        CheckGrounded();
        ProcessInput();
        UpdateMovement();
        UpdateAnimator();
        UpdateFacing();
    }

    private void CheckGrounded()
    {
        isGrounded = false;
        // co tutaj bedziemy robic
        int count = coll.Cast(Vector2.down, contactFilter, results, 0.1f);
        if(count > 0)
        {
            isGrounded = true;
        }
        Debug.Log("Is grounded: " + isGrounded);
    }

    private void UpdateFacing()
    {
        if(horizontalInput > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        if(horizontalInput < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
    }

    private void UpdateAnimator()
    {
        float currentSpeed = movementSpeed * horizontalInput;
        currentSpeed = Mathf.Abs(currentSpeed);
        animator.SetFloat("Speed", currentSpeed);
    }

    private void UpdateMovement()
    {
        Vector2 velocity = rb.velocity;
        velocity.x = horizontalInput * movementSpeed;

        if(isJumping == true)
        {
            velocity.y = jumpForce;
        }
        rb.velocity = velocity;
    }

    private void ProcessInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        isJumping = Input.GetKeyDown(KeyCode.Space) && isGrounded;
    }



}
