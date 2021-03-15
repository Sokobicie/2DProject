using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float horizontalInput = 0f;
    public float movementSpeed = 3f;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    // Start is called before the first frame update      
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
        UpdateMovement();
        UpdateAnimator();
        UpdateFacing();
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
        Vector3 velocity = new Vector3(movementSpeed, 0f);
        velocity = velocity * horizontalInput;
        velocity *= Time.deltaTime;
        transform.position += velocity;
    }

    private void ProcessInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
    }



}
