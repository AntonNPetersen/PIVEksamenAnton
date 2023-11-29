using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ClairMovement : MonoBehaviour
{
    // Variables for movement and jumping parameters
    public float moveSpeed = 5.0f;
    public float jumpForce = 8f;
    public int maxJumps = 2;
    public CapsuleCollider capsuleCollider;
    public Vector2 lastDirection = new Vector2(1, 0);
    public float floatControlForce = 2f;
    
    // Event for updating UI on jumps
    public delegate void JumpsUpdateHandler(int maxJumps, int remainingJumps);
    public event JumpsUpdateHandler OnJumpsUpdate;
    
    // Variables for movement control
    private Rigidbody rb;
    public int remainingJumps;
    private bool isGrounded;
    private Vector3 movement;
    [SerializeField] private LayerMask groundLayerMask;
    private PlayerState currentState;
    private bool isGroundedTimerActive = false;
    private float groundedTimer = 0.0f;
    private float groundedDelay = 0.5f;
    public Animator animator;
    //private bool isHoldingSpace = false;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize rigidbody and set initial state
        rb = GetComponent<Rigidbody>();
        currentState = PlayerState.Grounded;
        
        // Setting up reimainingjumps to be equal to maxjumps at the beginning
        remainingJumps = maxJumps;
        
        // Assign the Capsule Collider reference
        capsuleCollider = GetComponent<CapsuleCollider>();
    }
    
    // Enumeration representing player states
    public enum PlayerState
    {
        Grounded,
        Jumping,
        Floating
    }
    // Method to update the grounded status using raycasting
    void UpdateGroundedStatus()
    {
        // Raycast to check if the player is grounded
        isGrounded = Physics.CheckCapsule(capsuleCollider.bounds.center, new Vector3(capsuleCollider.bounds.center.x, capsuleCollider.bounds.min.y, capsuleCollider.bounds.center.z), capsuleCollider.radius * 0.9f, groundLayerMask);

        if (isGrounded)
        {
            Debug.Log("Raycast ground - Hit");
        }
        else
        {
            // Debug.Log("Raycast did not hit anything.");
            isGrounded = false;
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        // Invoke the OnJumpsUpdate event to update UI
        if (OnJumpsUpdate != null)
        {
            OnJumpsUpdate(maxJumps, remainingJumps);
        }
        // Input handling for movement
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
        // Update grounded status
        UpdateGroundedStatus();
        
        // State machine to handle different player states
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
    
    // Method to handle movement
    void HandleMovement(int X, int Y)
    {
        // Ground movement
        movement = new Vector3(Y * .7f, 0, X * .7f);
        movement.Normalize();
        rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed);
        
        // Animation control based on movement and state
        if (movement != Vector3.zero)
        {
            lastDirection = movement;
            transform.LookAt(transform.position + movement);
            
            //Walking anim is true
            if (isGrounded == true)
            {
                animator.SetBool("Walking", true);
                animator.SetBool("Idle", false);
                animator.SetBool("Jumping", false);
                animator.SetBool("Floating", false);
            }
        }
        else
        {   
            // Idle anim is true
            if (currentState == PlayerState.Grounded)
            {
                animator.SetBool("Walking", false);
                animator.SetBool("Idle", true);
                animator.SetBool("Jumping", false);
                animator.SetBool("Floating", false);
            }
        }
        // Jumping anim is true
        if (currentState == PlayerState.Jumping)
        {
            animator.SetBool("Walking", false);
            animator.SetBool("Idle", false);
            animator.SetBool("Jumping", true);
            animator.SetBool("Floating", false);
        }
    }
    
    // Method to handle grounded state
    private void GroundedUpdate(int X, int Y)
    {
        // Check for jump input to initiate a jump as well as checking if there is enough jumps
        if (Input.GetKeyDown(KeyCode.Space) && remainingJumps > 0)
        {
            // Call jump function to jump
            Jump();
            remainingJumps--;
            
            // Changes the players state to be jumping
            currentState = PlayerState.Jumping;
            // Reset the timer when starting the jump
            isGroundedTimerActive = false;
            groundedTimer = 0.0f;
            Debug.Log("Jump initiated");
            Debug.Log("Remaining Jumps: " + remainingJumps);
            return; // Exit early to avoid grounded movement logic after initiating jump
        }
        
        // Handle regular grounded movement from the function
        HandleMovement(X, Y);
        
        // Reset jumps and drag when grounded
        if (isGrounded)
        {
            rb.drag = 0f;
            remainingJumps = maxJumps;
        }
        
        
        Debug.Log("Remaining jumps for ground update" + remainingJumps);
        
        // Transition to Floating state when the jump is complete
        if (rb.velocity.y <= 0 && !isGrounded)
        {
            currentState = PlayerState.Floating;
            Debug.Log("floating is active");
        }
    }
    
    // Method to handle jumping state
    private void JumpingUpdate(int X, int Y)
    {
        // Handle movement during jumping
        HandleMovement(X, Y);
        Debug.Log("I am jumping");
        rb.drag = 0f;
        isGroundedTimerActive = true;
        
        // Check for additional jumps during the jump
        if (Input.GetKeyDown(KeyCode.Space) && remainingJumps > 0)
        {
            Jump();
            remainingJumps--;
            // Reset the timer when starting the jump
            isGroundedTimerActive = false;
            groundedTimer = 0.0f;
        }
        
        // Transition to Floating state when the jump is complete
        if (rb.velocity.y <= 0 && !isGrounded)
        {
            currentState = PlayerState.Floating;
            Debug.Log("floating is active");
        }
        // Activating a timer in order to not get into the grounded state too fast when perfoming a jump
        if (!isGroundedTimerActive)
        {
            groundedTimer += Time.deltaTime;

            // If the timer reaches the delay, and the the player is grounded, transition to Grounded state
            if (groundedTimer >= groundedDelay && isGrounded)
            {
                currentState = PlayerState.Grounded;
                Debug.Log("Is back on the ground. Remaining jumps: " + remainingJumps);
            }
        }
    }
    
    // Method to handle floating state
    private void FloatingUpdate(int X, int Y)
    {
        // Handle movement during floating
        HandleMovement(X, Y);
        Debug.Log("I am floating");
        
        // Floating logic
        if (Input.GetKeyDown(KeyCode.Space) && remainingJumps > 0)
        {
            Jump();
            remainingJumps--;
            currentState = PlayerState.Jumping;
        }
        
        // Handle airborne drag when space is held, which uses GetKey in order to use space key for a jump and float
        if (Input.GetKey(KeyCode.Space))
        {
            // Sets floating anim to true
            if (currentState == PlayerState.Floating)
            {
                animator.SetBool("Walking", false);
                animator.SetBool("Idle", false);
                animator.SetBool("Jumping", false);
                animator.SetBool("Floating", true);
            }
            rb.drag = 5f;
            // Airborne drag
            if (X != 0 || Y != 0)
            {
                if (rb.drag > 0)
                {
                    rb.drag = 5f;
                }
            }
            else
            {
                rb.drag -= Time.deltaTime * 3f;
                
            }
        }
        else
        {
            rb.drag = 0f;
        }
        

        // Transition to Grounded state when the player is back on the ground
        if (isGrounded)
        {
            currentState = PlayerState.Grounded;
        }
    }
    
    // Method to handle the jump action
    private void Jump()
    {
        // Reset downward velocity to zero before applying the upward force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        
        // Apply the jump force and update state
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        currentState = PlayerState.Jumping;
        
        // Invoke the OnJumpsUpdate event to update UI
        if (OnJumpsUpdate != null)
        {
            OnJumpsUpdate(maxJumps, remainingJumps);
        }
    }
    
    // Method to increase the maximum jumps
    public void IncreaseMaxJumps()
    {
        // Adds an additional max jump
        maxJumps++;
        Debug.Log("Max Jumps increased to: " + maxJumps);
        
        // Invoke the OnJumpsUpdate event to update UI
        if (OnJumpsUpdate != null)
        {
            OnJumpsUpdate(maxJumps, remainingJumps);
        }
    }
}
