using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
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
    CapsuleCollider collider;

    float originalHeight;

    RaycastHit slopeHit;

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

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        body.freezeRotation = true;
        collider = GetComponentInChildren<CapsuleCollider>();
        originalHeight = collider.height;
    }

    private void Update()
    {
        distanceToGround = GetComponentInChildren<Collider>().bounds.extents.y;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        MyInput();
        ControlDrag();

        if(Input.GetKeyDown(jumpKey) && isGrounded)
        {
            if(isSliding)
            {
                isSliding = false;
                StopSlide();
            }
            Jump();
        }
        else if (Input.GetKeyDown(slideKey) && isGrounded && !isSliding)
        {
            isSliding = true;
            Slide();
        }
        else if (Input.GetKeyDown(slideKey) && isGrounded && isSliding)
        {
            isSliding = false;
            StopSlide();
        }      

        if(isSliding && body.velocity.magnitude < 3)
        {
            isSliding = false;
            StopSlide();
        }

        if(isSliding && OnSlope())
        {
            SpeedUpSlopeSlide();
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
        //print(body.velocity.magnitude);
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    void Jump()
    {
        body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);
        body.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void Slide()
    {
        groundDrag = 2f;
        collider.height = reducedHeight;
        if(OnSlope())
        {
            StartSlopeSlide();
        } else
        {
            body.AddForce(moveDirection.normalized * slideBoost * movementMultiplier, ForceMode.VelocityChange);
        }
        
    }

    void StartSlopeSlide()
    {
        body.AddForce(slopeMoveDirection * slideBoost * movementMultiplier , ForceMode.Acceleration);
        slopeDirection = slopeMoveDirection;
    }

    void SpeedUpSlopeSlide()
    {
        slopeSlideAccumulator += .5f;
        body.AddForce(slopeDirection * slideBoost * slopeSlideAccumulator, ForceMode.Acceleration);
    }

    void StopSlide()
    {
        slopeSlideAccumulator = 0f;
        groundDrag = 6f;
        collider.height = originalHeight;
    }

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

    private void FixedUpdate()
    {
        print(isSliding);
        if(!isSliding)
        {
            MovePlayer();
        }
    }

    void MovePlayer()
    {
        if(isGrounded && !OnSlope())
        {
            body.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if(isGrounded && OnSlope())
        {
            body.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if(!isGrounded)
        {
            body.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
        
    }
}
