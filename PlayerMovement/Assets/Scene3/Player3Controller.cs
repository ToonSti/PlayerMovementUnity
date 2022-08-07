using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3Controller : MonoBehaviour
{
    [SerializeField] private LayerMask isGround;                                // define what is ground in layers
    private float groundAngle;                                                  // the angle of the ground from the player
    private float groundDist;                                                   // distance the player should be from the player
    [SerializeField] private float maxSlopeLimit = 60;                          // angle from which the player should not be able to move from

    [SerializeField] private float horizontalSpeed;                             // horizontal speed multiplier, make the object go faster or slower
    [SerializeField] private float walkSpeed = 6;                               // speed multiplier when walking
    [SerializeField] private float sprintSpeed = 9;                             // speed multiplier when sprinting
    [SerializeField] private float crouchSpeed = 4;                             // speed multiplier when crouching
    [SerializeField] private float crouchHeight = 1.5f;                         // height of the CapsuleCollider when crouching

    [SerializeField] private float jumpSpeed = 7;                               // amount of force for vertical movement (jumping)
    [SerializeField] private bool airControl = false;                           // decide if the player can be controlled in air, make sure to change this option before running the project

    private float smoothInputSpeed;                                             // time of smoothing in the SmoothDamp
    [SerializeField] private float smoothInputSpeedAir = 1.5f;                  // time of smoothing in the SmoothDamp in air
    [SerializeField] private float smoothInputSpeedGround = .2f;                // time of smoothing in the SmoothDamp on ground

    private Rigidbody rb;                                                       // variable to controll the Rigidbody
    private CapsuleCollider cc;                                                 // variable to controll the Capsule Collider
    private float capsRadius;                                                   // radius of the Capsule Collider
    private Vector3 capsBottom;                                                 // the bottom of the Capsule collider
    private float capsHeight;                                                   // general height of the capsulecollider

    private Vector3 currentInputVector;                                         // current smoothened input vector
    private Vector3 smoothInputVelocity;                                        // requirement for the SmoothDamp, gets populated in the function
    private Vector3 horizontalMove;                                             // variable to store the normalized horizontal inputs

    private float mousePosX;                                                    // mouse position on the X axis
    [Range(10f, 100f)]
    [SerializeField] private float sensitivity = 100;                           // sensitivity slider

    // function called at the very start of the game, before start
    private void Awake()
    {
        // assign the Rigidbody component to the variable
        rb = GetComponent<Rigidbody>();
        // assign the Capsule Collider component to the variable
        cc = GetComponent<CapsuleCollider>();
        // save the radius of the Capsule Collider to a variable
        capsRadius = cc.radius;
        // save the height of the Capsule Collider to a variable
        capsHeight = cc.height;
        // multiply the sensitivity 
        sensitivity *= 100;
        // set the horizontalSpeed to the walkSpeed as a starting value
        horizontalSpeed = walkSpeed;
    }

    // function called every frame of the game
    public void Move(Vector3 input, bool jump, bool sprint, bool crouch, float rotationX)
    {
        Debug.Log(IsGrounded());
        // Debug, show where the groundcheck checks
        Debug.DrawRay(capsBottom, -transform.up, Color.green, .1f);
        
        // normalize the inputs and store them
        horizontalMove = input.normalized;

        if (sprint && !crouch)                      // if sprinting and not crouching 
        {
            horizontalSpeed = sprintSpeed;          // set the speedmultiplier to the sprintspeed
        }
        if (crouch && !sprint)                      // if crouching and not sprinting
        {
            horizontalSpeed = crouchSpeed;          // set the speedmultiplier to the crouchspeed
            cc.height = crouchHeight;               // let the player crouch by changing the capsule collider height
        }
        if (sprint && crouch)                       // if crouching and sprinting
        {
            horizontalSpeed = walkSpeed;            // set the speedmultiplier to the walkspeed
        }
        if (!sprint && !crouch)                     // if not sprinting or crouching
        {
            horizontalSpeed = walkSpeed;            // set the speedmultiplier to the walkspeed
        }
        if (!crouch)                                // if not crouching
        {
            cc.height = capsHeight;                 // set the capsule collider height back to it's original state
        }

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
            // applying horizontal velocity to the Rigidbody multiplied by the speed in local space
            rb.velocity = transform.TransformDirection(new Vector3(currentInputVector.x * horizontalSpeed, rb.velocity.y, currentInputVector.z * horizontalSpeed));
            
        }

        // run code jump == true and IsGrounded == true         (because of the movementscript, the IsGrounded in this statement may be unnecessary)
        if (jump && IsGrounded())
        {
            // applying vertical velocity to the Rigidbody multiplied by the jumpSpeed
            rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
        }

        // save the mouse movement in mousePosX
        mousePosX += rotationX * sensitivity * Time.deltaTime;
        // rotate the player with the saved mousemovement
        transform.rotation = Quaternion.Euler(0f, mousePosX, 0f);
    }

    // function called everytime IsGrounded() is mentioned in a script, a boolean value is returned
    public bool IsGrounded()
    {
        // calculate the capsule bottom in worldspace, because the ray needs to be checked in world space
        capsBottom = transform.TransformPoint(cc.center - Vector3.up * cc.height / 2f);
        // make a ray from .01 above the capsBottom pointing down
        var Ground = new Ray(capsBottom + transform.up * .01f, -transform.up);
        // get the data from the Ground ray
        RaycastHit groundData;
        // if the Ground ray hits something withing 5 times the capsuleradius that is considered ground in the layermask
        if (Physics.Raycast(Ground, out groundData, capsRadius * 5f, isGround))
        {
            // calculate the angle  that the ground is from the player
            groundAngle = Vector3.Angle(groundData.normal, transform.up);
            // if the angle is lower than the set slopelimit, allow the player to be grounded
            if (groundAngle < maxSlopeLimit)
            {
                // calculate the distance the Ground ray should be poinding from the ground with the calculated angle, leaving a .01 margin (see ray Ground) using a trigonometric function
                groundDist = capsRadius / Mathf.Cos(Mathf.Deg2Rad * groundAngle) - capsRadius + .02f;
                // if the measured distance is smaller than the calculated distance
                if (groundData.distance < groundDist)
                {
                    // return true, so that the player is grounded
                    return true;
                }
                // return false in any other case
                else{
                    return false;
                }
            }else{
                return false;
            }
        }else{
            return false;
        }
    }
}
