using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;

    [SerializeField] private float groundDrag;

    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpCooldown = 0.1f;
    [SerializeField] private float airMultiplier = 2f;
    private bool canJump = false;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;


    [SerializeField] private Transform orientation;

    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool grounded;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;

    private Rigidbody rigidbody;


    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.freezeRotation = true;

        canJump = true;

    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);

        MyInput();
        SpeedControl();

        //if(grounded)
        //{
        //    canJump = true;
        //}
        //else
        //{
        //    canJump = false;

        //}

        rigidbody.drag = grounded ? groundDrag : 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && grounded && canJump)
        {
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(grounded)
        {
            rigidbody.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rigidbody.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Acceleration);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rigidbody.velocity = new Vector3(limitedVelocity.x, rigidbody.velocity.y, limitedVelocity.z);
        }
    }

    private void Jump()
    {
        canJump = false;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
        rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        canJump = true;
    }
}
