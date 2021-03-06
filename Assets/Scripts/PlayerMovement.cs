using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Variables
    [SerializeField] Transform orientation;

    [Header("Movement")]
    public float moveSpeed = 6f;
    float movementMultiplier = 10f;
    [SerializeField] float airMultiplier = 0.5f;

    [Header("Jumping")]
    public float jumpForce = 5f;

    [Header("Sliding")]
    [SerializeField] float slideBoost = 10f;
    [SerializeField] float reducedHeight;
    bool isSliding = false;
    bool slideQueued = false;
    float slopeSlideAccumulator = 0;

    [Header("Drag")]
    [SerializeField] float groundDrag = 6f;
    [SerializeField] float airDrag = 2f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode slideKey = KeyCode.LeftControl;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    bool isGrounded;
    float groundDistance = 0.4f;

    float horizontalMovement;
    float verticalMovement;

    float distanceToGround;

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;
    Vector3 slopeDirection;

    Rigidbody body;
    CapsuleCollider playerCollider;

    float originalHeight;

    RaycastHit slopeHit;
    RaycastHit slopeSlideHit;

    /// <summary>
    /// Determines whether or not the Player is on a slope
    /// </summary>
    /// <returns>bool signifying if the Player is on a slope</returns>
    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, distanceToGround + 0.5f))
        {
            if(slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Gets necessary references and sets certain Player conditions
    /// </summary>
    private void Start()
    {
        body = GetComponent<Rigidbody>();
        body.freezeRotation = true;
        playerCollider = GetComponentInChildren<CapsuleCollider>();
        originalHeight = playerCollider.height;
    }

    /// <summary>
    /// Handles all Player input pertaining to movement
    /// </summary>
    private void Update()
    {
        distanceToGround = GetComponentInChildren<Collider>().bounds.extents.y;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        MyInput();
        ControlDrag();

        // Jumping
        if(Input.GetKeyDown(jumpKey) && isGrounded)
        {
            if(isSliding)
            {
                isSliding = false;
                StopSlide();
            }
            Jump();
        }
        // Sliding
        else if(Input.GetKeyDown(slideKey) && isGrounded && !isSliding)
        {
            isSliding = true;
            Slide();
        }
        // Jumping out of slide
        else if(Input.GetKeyDown(slideKey) && isGrounded && isSliding)
        {
            isSliding = false;
            StopSlide();
        }      
        
        // Queue slide from air
        if(Input.GetKeyDown(slideKey) && !isGrounded && !isSliding)
        {
            slideQueued = !slideQueued;
        }
        // Detect air to slide
        if(isGrounded && slideQueued)
        {
            isSliding = true;
            slideQueued = false;
            AirToSlide();
        }

        // Stop slide if Player slows to a certain speed
        if(isSliding && body.velocity.magnitude < 3 && !OnSlope())
        {
            isSliding = false;
            StopSlide();
        }

        // Detect slope slide
        if(isSliding && OnSlope())
        {
            Physics.Raycast(transform.position, -transform.up, out slopeSlideHit);
            Vector3 left = Vector3.Cross(slopeSlideHit.normal, Vector3.up);
            Vector3 slope = Vector3.Cross(slopeSlideHit.normal, left);
            slopeDirection = slope;
            SpeedUpSlopeSlide();
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    /// <summary>
    /// Executes Player movement
    /// </summary>
    private void FixedUpdate()
    {
        if (!isSliding || !isGrounded)
        {
            MovePlayer();
        }
    }

    /// <summary>
    /// Adds forces to execute Player's WASD movements
    /// </summary>
    void MovePlayer()
    {
        if (isGrounded && !OnSlope())
        {
            body.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            body.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            body.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }

    }

    /// <summary>
    /// Detects WASD input from Player and sets move direction accordingly
    /// </summary>
    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    /// <summary>
    /// Add force to Player to make them jump
    /// </summary>
    void Jump()
    {
        body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);
        body.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    /// <summary>
    /// Reduce Player height and add force to Player to make them slide
    /// </summary>
    void Slide()
    {
        groundDrag = 2f;
        playerCollider.height = reducedHeight;
        if(OnSlope())
        {
            StartSlopeSlide();
        } else
        {
            body.AddForce(moveDirection.normalized * slideBoost * movementMultiplier, ForceMode.VelocityChange);
        }
        
    }

    /// <summary>
    /// Handles direction and force when the Player executes a slide from the air
    /// </summary>
    void AirToSlide()
    {
        groundDrag = 2f;
        playerCollider.height = reducedHeight;

        Vector3 momentumDirection = body.velocity;
        body.AddForce(momentumDirection.normalized * slideBoost * 8, ForceMode.VelocityChange);
    }

    /// <summary>
    /// Begins the Player's slide down a slope
    /// </summary>
    void StartSlopeSlide()
    {
        body.AddForce(slopeMoveDirection * slideBoost * movementMultiplier, ForceMode.Acceleration);
    }

    /// <summary>
    /// Speeds up the Player's slope slide over time
    /// </summary>
    void SpeedUpSlopeSlide()
    {
        slopeSlideAccumulator += .75f;
        body.AddForce(slopeDirection * slideBoost * slopeSlideAccumulator, ForceMode.Acceleration);
    }

    /// <summary>
    /// Returns Player to normal conditions and ends slide
    /// </summary>
    void StopSlide()
    {
        slopeSlideAccumulator = 0f;
        groundDrag = 6f;
        playerCollider.height = originalHeight;
    }

    /// <summary>
    /// Ends Player's slide if they use a jump pad
    /// </summary>
    /// <param name="collision">Collision object</param>
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Jump Pad" && isSliding)
        {
            isSliding = false;
            StopSlide();
        }
    }

    /// <summary>
    /// Changes drag depending on if Player is grounded or in the air
    /// </summary>
    void ControlDrag()
    {
        if(isGrounded)
        {
            body.drag = groundDrag;
        }
        else if (!isGrounded)
        {
            body.drag = airDrag;
            body.AddForce(Physics.gravity);
        }
    }
}
