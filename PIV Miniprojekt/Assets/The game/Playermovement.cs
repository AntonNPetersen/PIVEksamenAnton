using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Playermovement : MonoBehaviour
{
    private float turnSmoothVelocity;
    private Vector2 moveInput;
    private Rigidbody rb;
    private int remainingJumps;
    private bool isGrounded;
    private bool isGliding;

    public float moveSpeed = 6f;
    public float turnSmoothTime = 0.1f;
    public float jumpForce = 4f;
    public float glideDescentSpeed = 0.5f;
    public InputActionReference jumpAction;
    public InputActionReference glideAction;
    public int maxJumps = 2;
    public Transform groundCheck;
    public Vector2 lastDirection = new Vector2(1,0);
   

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpAction.action.Enable();
        glideAction.action.Enable();
        jumpAction.action.performed += context => Jump();
        remainingJumps = maxJumps;
    }

    private void Update()
    {
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);
        movement.Normalize(); // Normalize the movement vector to ensure consistent speed in all directions
        movement *= moveSpeed * Time.deltaTime;
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, 0.1f);

        // Rotate the character to face the direction of movement
        if (movement != Vector3.zero)
        {
            float targetRotation = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
            float smoothRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothRotation, 0f);
        }

        rb.MovePosition(transform.position + movement);

  
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            if(moveInput.x == 0 && moveInput.y == 0)
            {
                moveInput = lastDirection;
                if (rb.drag > 0)
                {
                    rb.drag -= Time.deltaTime * 2f;
                }
            }
            else
            {
                if (rb.drag < 10)
                {
                    rb.drag += Time.deltaTime * 3f;
                }
            }
        }

        else
        {
            rb.drag = 0f;
        }
        
        if (isGrounded)
        {
            remainingJumps = maxJumps;
            
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Read the movement input from the Input System
        moveInput = context.ReadValue<Vector2>();
        if (moveInput.x != 0 || moveInput.y != 0)
        {
            lastDirection = moveInput;
        }
    }

    private void Jump()
    {
        //if (Mathf.Abs(rb.velocity.y) < 0.01f)
        if (remainingJumps > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            remainingJumps--;
        }
    }

}