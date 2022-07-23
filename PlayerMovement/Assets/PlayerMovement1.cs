using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement1 : MonoBehaviour
{
    [SerializeField] private LayerMask isGround;            // define what is ground in layers
	[SerializeField] private Transform groundCheck;         // empty gameObject with transform position for checking the floor
    
    public float horizontalSpeed = 2;                       // horizontal speed multiplier, make the object go faster or slower
    public float jumpSpeed = 2;                             // amount of force for vertical movement (jumping)
    public float groundRadius = .1f;                        // radius that the groundCheck should search in for isGround
    public bool airControl = false;                         // decide if the player can be controlled in air
    public float smoothInputSpeed = .2f;                    // amount of smoothing in the SmoothDamp
    public float rotationSpeed;                             // amount of time used for the rotating of the player

    private Rigidbody rb;                                   // variable to controll the Rigidbody
    
    private Vector3 currentInputVector;                     // current smoothened imput vector
    private Vector3 smoothInputVelocity;                    // requirement for the SmoothDamp, gets populated in the function


    // function called at the very start of the game, before start
    void Awake()
    {
        // assign the Rigidbody component to the variable
        rb = GetComponent<Rigidbody>();
    }

    // function called every frame of the game
    void Update()
    {
        // get the horizontal inputs and put them in a vector 3
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        // Smoothen the buildup of the vector 3 using SmoothDamp
        currentInputVector = Vector3.SmoothDamp(currentInputVector, input, ref smoothInputVelocity, smoothInputSpeed);
        
        // run code if IsGrounded == true of airControl == true
        if (IsGrounded() || airControl)
        {
            // applying horizontal velocity to the Rigidbody multiplied by the speed
            rb.velocity = new Vector3(currentInputVector.x * horizontalSpeed, rb.velocity.y, currentInputVector.z * horizontalSpeed);
        }
        
        // run code Jump is pressed and IsGrounded == true
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            // applying vertical velocity to the Rigidbody multiplied by the jumpSpeed
            rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
        }

        // if there are inputs, execute the code
        if (input != Vector3.zero)
        {
            // makes the needed rotation
            Quaternion toRotation = Quaternion.LookRotation(input, Vector3.up);
            // rotates the player to the needed rotation over a period of time
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // function called everytime IsGrounded() is mentioned in the code
    bool IsGrounded()
    {
        // check for isGround at a groundRadius radius around the groundCheck
        return Physics.CheckSphere(groundCheck.position, groundRadius, isGround);
    }
}
