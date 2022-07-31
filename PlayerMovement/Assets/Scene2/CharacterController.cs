using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private LayerMask isGround;                                // define what is ground in layers
    [SerializeField] private Transform groundCheck;                             // empty gameObject with transform position for checking the floor

    [SerializeField] private float horizontalSpeed = 2;                         // horizontal speed multiplier, make the object go faster or slower
    [SerializeField] private float jumpSpeed = 2;                               // amount of force for vertical movement (jumping)
    [SerializeField] private float groundRadius = .1f;                          // radius that the groundCheck should search in for isGround
    [SerializeField] private bool airControl = false;                           // decide if the player can be controlled in air, make sure to change this option before running the project
    [SerializeField] private float rotationSpeed;                               // amount of time used for the rotating of the player

    private float smoothInputSpeed;                                             // time of smoothing in the SmoothDamp
    [SerializeField] private float smoothInputSpeedAir = 1.5f;                  // time of smoothing in the SmoothDamp in air
    [SerializeField] private float smoothInputSpeedGround = .2f;                // time of smoothing in the SmoothDamp on ground

    private Rigidbody rb;                                                       // variable to controll the Rigidbody

    private Vector3 currentInputVector;                                         // current smoothened input vector
    private Vector3 smoothInputVelocity;                                        // requirement for the SmoothDamp, gets populated in the function
    public Vector3 horizontalMove;                                              // variable to store the normalized inputs

    [SerializeField] private GameObject Camera;


    // function called at the very start of the game, before start
    private void Awake()
    {
        // assign the Rigidbody component to the variable
        rb = GetComponent<Rigidbody>();
    }

    // function called every frame of the game
    public void Move(Vector3 input, bool jump)
    {
        // normalize the inputs and store them
        horizontalMove = input.normalized;
        
        // if the player is not touching the ground
        if (!IsGrounded())
        {
            // increase the smoothtime in air for a smooth landing
            smoothInputSpeed = smoothInputSpeedAir;
        }
        // else, so if the player is touching the ground
        else
        {
            // changing the smoothtime back to the standard
            smoothInputSpeed = smoothInputSpeedGround;
        }

        // Smoothen the buildup of the vector 3 using SmoothDamp
        currentInputVector = Vector3.SmoothDamp(currentInputVector, horizontalMove, ref smoothInputVelocity, smoothInputSpeed);

        // run code if IsGrounded == true or airControl == true
        if (IsGrounded() || airControl)
        {
            // applying horizontal velocity to the Rigidbody multiplied by the speed
            rb.velocity = new Vector3(currentInputVector.x * horizontalSpeed, rb.velocity.y, currentInputVector.z * horizontalSpeed);
        }

        // run code jump == true and IsGrounded == true         (because of the movementscript, the IsGrounded in this statement may be unnecessary)
        if (jump && IsGrounded())
        {
            // applying vertical velocity to the Rigidbody multiplied by the jumpSpeed
            rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
            jump = false;
        }

        // if there are inputs, execute the code
        if (horizontalMove != Vector3.zero)
        {
            // makes the wanted rotation
            Quaternion toRotation = Quaternion.LookRotation(horizontalMove, Vector3.up);
            // rotates the player to the needed rotation over a period of time
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // function called everytime IsGrounded() is mentioned in a script, a boolean value is returned
    public bool IsGrounded()
    {
        // check for isGround at a groundRadius radius around the groundCheck
        return Physics.CheckSphere(groundCheck.position, groundRadius, isGround);
    }

    public void CameraMove()
    {

    }
}
