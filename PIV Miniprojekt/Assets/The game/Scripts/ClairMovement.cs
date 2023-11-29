using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ClairMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float jumpForce = 8f;
    public int maxJumps = 2;
    public CapsuleCollider capsuleCollider;
    public Vector2 lastDirection = new Vector2(1, 0);
    public float floatControlForce = 2f;
    public delegate void JumpsUpdateHandler(int maxJumps, int remainingJumps);
    public event JumpsUpdateHandler OnJumpsUpdate;

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
        rb = GetComponent<Rigidbody>();
        currentState = PlayerState.Grounded;
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

    void UpdateGroundedStatus()
    {
        Vector3 groundCheckPos = transform.position - new Vector3(0, capsuleCollider.height, 0);
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
            isGrounded = false;
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
        /*Vector3 groundCheckPos = transform.position - new Vector3(0, capsuleCollider.height / 2, 0);
        Debug.DrawRay(groundCheckPos, Vector3.down * 0.1f, Color.red);  // Debug visualization
        RaycastHit[] hits = Physics.RaycastAll(groundCheckPos, Vector3.down, 0.1f);
        isGrounded = hits.Any(hit => hit.collider.CompareTag("Ground"));*/
        // Update the UI when jumps change
        if (OnJumpsUpdate != null)
        {
            OnJumpsUpdate(maxJumps, remainingJumps);
        }
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
    void HandleMovement(int X, int Y)
    {
        // Ground movement
        movement = new Vector3(Y * .7f, 0, X * .7f);
        movement.Normalize();
        //rb.velocity = movement * moveSpeed * Time.deltaTime;
        rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed);
        
        if (movement != Vector3.zero)
        {
            lastDirection = movement;
            transform.LookAt(transform.position + movement);
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
            if (currentState == PlayerState.Grounded)
            {
                animator.SetBool("Walking", false);
                animator.SetBool("Idle", true);
                animator.SetBool("Jumping", false);
                animator.SetBool("Floating", false);
            }
        }
        if (currentState == PlayerState.Jumping)
        {
            animator.SetBool("Walking", false);
            animator.SetBool("Idle", false);
            animator.SetBool("Jumping", true);
            animator.SetBool("Floating", false);
        }
    }
    private void GroundedUpdate(int X, int Y)
    {
        if (Input.GetKeyDown(KeyCode.Space) && remainingJumps > 0)
        {
            Jump();
            remainingJumps--;
            currentState = PlayerState.Jumping;
            // Reset the timer when starting the jump
            isGroundedTimerActive = false;
            groundedTimer = 0.0f;
            Debug.Log("Jump initiated");
            Debug.Log("Remaining Jumps: " + remainingJumps);
            return; // Exit early to avoid grounded movement logic after initiating jump
        }

        HandleMovement(X, Y);
        
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

    private void JumpingUpdate(int X, int Y)
    {
        HandleMovement(X, Y);
        Debug.Log("I am jumping");
        rb.drag = 0f;
        isGroundedTimerActive = true;

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
        
        // Transition to Grounded state if the player is back on the ground
        /*if (isGrounded)
        {
            currentState = PlayerState.Grounded;
            Debug.Log("Is back on the ground. Remaining jumps: " + remainingJumps);
        }*/
        // Start the timer when transitioning to Floating state
        if (!isGroundedTimerActive)
        {
            groundedTimer += Time.deltaTime;

            // If the timer reaches the delay, transition to Grounded state
            if (groundedTimer >= groundedDelay && isGrounded)
            {
                currentState = PlayerState.Grounded;
                Debug.Log("Is back on the ground. Remaining jumps: " + remainingJumps);
            }
        }
    }

    private void FloatingUpdate(int X, int Y)
    {
        HandleMovement(X, Y);
        Debug.Log("I am floating");
        
        // Floating logic
        if (Input.GetKeyDown(KeyCode.Space) && remainingJumps > 0)
        {
            Jump();
            remainingJumps--;
            currentState = PlayerState.Jumping;
            //rb.AddForce(Vector3.up * floatControlForce, ForceMode.Acceleration);
        }

        if (Input.GetKey(KeyCode.Space))
        {
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

    private void Jump()
    {
        // Reset downward velocity to zero before applying the upward force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        currentState = PlayerState.Jumping;
        
        if (OnJumpsUpdate != null)
        {
            OnJumpsUpdate(maxJumps, remainingJumps);
        }
    }

    public void IncreaseMaxJumps()
    {
        maxJumps++;
        Debug.Log("Max Jumps increased to: " + maxJumps);
        
        if (OnJumpsUpdate != null)
        {
            OnJumpsUpdate(maxJumps, remainingJumps);
        }
    }
}
