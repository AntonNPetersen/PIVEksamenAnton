using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float jumpForce = 8f;
    public int maxJumps = 2;
    public CapsuleCollider capsuleCollider;
    public Vector2 lastDirection = new Vector2(1, 0);
    public float floatControlForce = 2f;

    private Rigidbody rb;
    private int remainingJumps;
    private bool isGrounded;
    private Vector3 movement;
    [SerializeField] private LayerMask groundLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        remainingJumps = maxJumps;
        
        // Assign the Capsule Collider reference
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    public enum PlayerState
    {
        Grounded,
        Jumping,
        Floating
    }

    private PlayerState currentState = PlayerState.Grounded;
    
    void UpdateGroundedStatus()
    {
        float groundCheckDistance = 0.1f; // Adjust this based on your game's scale

        Vector3 groundCheckPos = transform.position - new Vector3(0, capsuleCollider.height / 2, 0);
        Debug.DrawRay(groundCheckPos, Vector3.down * 0.1f, Color.red);  // Debug visualization
        
        isGrounded = Physics.CheckCapsule(capsuleCollider.bounds.center, new Vector3(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y, capsuleCollider.bounds.center.z), capsuleCollider.radius * 0.9f, groundLayerMask);
        
        
        //RaycastHit[] hits = Physics.RaycastAll(groundCheckPos, Vector3.down, 0.1f, groundLayerMask);

        if (isGrounded)
        {
            Debug.Log("Raycast ground - Hit");
        }
        else
        {
            // Debug.Log("Raycast did not hit anything.");
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
        /*Vector3 groundCheckPos = transform.position - new Vector3(0, capsuleCollider.height / 2, 0);
        Debug.DrawRay(groundCheckPos, Vector3.down * 0.1f, Color.red);  // Debug visualization
        RaycastHit[] hits = Physics.RaycastAll(groundCheckPos, Vector3.down, 0.1f);
        isGrounded = hits.Any(hit => hit.collider.CompareTag("Ground"));*/
        
        int X = 0;
        int Y = 0;
        if (Input.GetKey(KeyCode.W))
        {
            X = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            X = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Y = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Y = 1;
        }
        
        UpdateGroundedStatus();
        
        switch (currentState)
        {
            case PlayerState.Grounded:
                GroundedUpdate(X, Y);
                break;
            case PlayerState.Jumping:
                JumpingUpdate(X, Y);
                break;
            case PlayerState.Floating:
                FloatingUpdate(X, Y);
                break;
        }
    }

    void GroundedUpdate(int X, int Y)
    {
        // Ground movement
        movement = new Vector3(Y * .7f, 0, X * .7f);
        movement.Normalize();
        //rb.velocity = movement * moveSpeed * Time.deltaTime;
        rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed) * Time.deltaTime;
        remainingJumps = maxJumps;

        if (movement != Vector3.zero)
        {
            lastDirection = movement;
            transform.LookAt(transform.position + movement);
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump();
                remainingJumps--;
                Debug.Log("Jump initiated");
                Debug.Log("Remaining Jumps: " + remainingJumps);
            }
            else if (remainingJumps > 0)
            {
                Jump();
                remainingJumps--;
                Debug.Log("Jump initiated");
                Debug.Log("Remaining Jumps: " + remainingJumps);
            }
            
        }
    }

    void JumpingUpdate(int X, int Y)
    {
        movement = new Vector3(Y, 0, X);
        movement.Normalize();
        rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed);

        // Airborne drag
        if (X != 0 || Y != 0)
        {
            if (rb.drag > 0)
            {
                rb.drag -= Time.deltaTime * 3f;
            }
        }
        else
        {
            rb.drag = 5f;
        }

        // Look at the direction of movement
        if (movement != Vector3.zero)
        {
            transform.LookAt(transform.position + movement);
        }

        // Transition to Floating state when the jump is complete
        if (rb.velocity.y <= 0 && !isGrounded)
        {
            currentState = PlayerState.Floating;
            Debug.Log("floating is active");
        }
        
        // Transition to Grounded state if the player is back on the ground
        if (isGrounded)
        {
            currentState = PlayerState.Grounded;
            Debug.Log("Is back on the ground. Remaining jumps: " + remainingJumps);
        }
        
        /* Transition to Grounded state if the player is back on the ground
        if (isGrounded)
        {
            currentState = PlayerState.Grounded;
            remainingJumps = maxJumps;
            isJumping = false;
            Debug.Log("is back on ground" + remainingJumps);
        }*/
    }

    void FloatingUpdate(int X, int Y)
    {
        // Floating logic
        if (Input.GetKeyDown(KeyCode.Space) && remainingJumps > 0)
        {
            Jump();
            remainingJumps--;
            currentState = PlayerState.Jumping;
            //rb.AddForce(Vector3.up * floatControlForce, ForceMode.Acceleration);
        }

        // Apply velocity for horizontal movement
        Vector3 horizontalVelocity = new Vector3(Y * .7f, 0, X * .7f); // Adjusted to match player input
        horizontalVelocity.Normalize();
        rb.velocity = new Vector3(horizontalVelocity.x * moveSpeed, rb.velocity.y, horizontalVelocity.z * moveSpeed);

        // Update lastDirection for rotation
        if (horizontalVelocity != Vector3.zero)
        {
            lastDirection = horizontalVelocity;
            transform.LookAt(transform.position + new Vector3(lastDirection.x, 0, lastDirection.y));
        }

        // Limit vertical speed to prevent flying upwards
        float verticalSpeed = Mathf.Clamp(rb.velocity.y, -floatControlForce, floatControlForce);
        rb.velocity = new Vector3(rb.velocity.x, verticalSpeed, rb.velocity.z);

        // Transition to Grounded state when the player is back on the ground
        if (isGrounded)
        {
            currentState = PlayerState.Grounded;
            
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        currentState = PlayerState.Jumping;
    }
}
