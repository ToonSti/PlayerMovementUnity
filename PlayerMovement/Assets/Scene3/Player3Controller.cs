using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3Controller : MonoBehaviour
{
    public LayerMask isGround;                                                  // define what is ground in layers
    [SerializeField] private GameObject doubleCeiling;                          // backup ceilingcheck
    [SerializeField] private GameObject camLookAt;
    private float groundAngle;                                                  // the angle of the ground from the player
    private float groundDist;                                                   // distance the player should be from the player
    [SerializeField] private float maxSlopeLimit = 60;                          // angle from which the player should not be able to move from

    private float horizontalSpeed;                                              // horizontal speed multiplier, make the object go faster or slower
    [SerializeField] private float walkSpeed = 6;                               // speed multiplier when walking
    [SerializeField] private float sprintSpeed = 9;                             // speed multiplier when sprinting
    [SerializeField] private float crouchSpeed = 4;                             // speed multiplier when crouching
    [SerializeField] private float playerCrouchHeight = .75f;                   // player height when crouching (value between >0 and 1)
    [SerializeField] private float crouchLerp = .2f;                            // speed the player crouches at
    private float currentCrouchLerp;                                            // current speed the player crouches at

    [SerializeField] private float jumpSpeed = 7;                               // amount of force for vertical movement (jumping)
    [SerializeField] private bool airControl = false;                           // decide if the player can be controlled in air, make sure to change this option before running the project

    private float smoothInputSpeed;                                             // time of smoothing in the SmoothDamp
    [SerializeField] private float smoothInputSpeedAir = 1.5f;                  // time of smoothing in the SmoothDamp in air
    [SerializeField] private float smoothInputSpeedGround = .2f;                // time of smoothing in the SmoothDamp on ground

    private Rigidbody rb;                                                       // variable to controll the Rigidbody
    private CapsuleCollider cc;                                                 // variable to controll the Capsule Collider
    private Vector3 capsBottom;                                                 // the bottom of the Capsule collider
    private Vector3 capsTop;                                                    // the top of the capsule collider
    private float playerHeight = 1;                                             // height of the player transform (1 is normal)

    private Vector3 currentInputVector;                                         // current smoothened input vector
    private Vector3 smoothInputVelocity;                                        // requirement for the SmoothDamp, gets populated in the function
    private Vector3 horizontalMove;                                             // variable to store the normalized horizontal inputs

    private float mousePosX;                                                    // mouse position on the X axis
    public float mousePosY;
    [Range(10f, 100f)]
    public float sensitivity;                                                   // sensitivity slider

    // function called at the very start of the game, before start
    private void Awake()
    {
        // assign the Rigidbody component to the variable
        rb = GetComponent<Rigidbody>();
        // assign the Capsule Collider component to the variable
        cc = GetComponent<CapsuleCollider>();
    }

    // Update is called at the start
    private void Start()
    {
        // set the horizontalSpeed to the walkSpeed as a starting value
        horizontalSpeed = walkSpeed;
        // set the smoothInputSpeed to the Ground value as a starting value
        smoothInputSpeed = smoothInputSpeedGround;
        // set current CrouchLerp to the standard value as a starting value
        currentCrouchLerp = crouchLerp;
    }

    // function called every frame of the game
    public void Move(float inputX, float inputZ, bool jump, bool sprint, bool crouch, float rotationX, float rotationY)
    {
        
        // Debug, show where the groundcheck and ceilingcheck checks
        Debug.DrawRay(capsBottom + transform.up * .01f, -transform.up * cc.radius * 5, Color.green, .1f);
        Debug.DrawRay(capsTop, transform.up * .5f, Color.green, .1f);

        // if the doublecheck ceilingcheck hits something and the player is not crouching
        if (Physics.CheckSphere(doubleCeiling.transform.position, cc.radius + .2f, isGround) && !crouch)
        {
            // set the currentcrouchlerp to 0, so the player stops uncrouching
            currentCrouchLerp = 0f;
        }
        // else, so of the doublecheck ceilingcheck hits nothing and the player is crouching
        else
        {
            // set the currentCrouchLerp to the usual state
            currentCrouchLerp = crouchLerp;
        }

        if (sprint && !crouch)                      // if sprinting and not crouching 
        {
            horizontalSpeed = sprintSpeed;          // set the speedmultiplier to the sprintspeed
        }
        if (crouch && !sprint)                      // if crouching and not sprinting
        {
            horizontalSpeed = crouchSpeed;          // set the speedmultiplier to the crouchspeed

            // let the player crouch by changing the current playerscale to the crouchscale using lerp
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, playerCrouchHeight, 1), currentCrouchLerp);
        }
        if (sprint && crouch)                       // if crouching and sprinting
        {
            horizontalSpeed = walkSpeed;            // set the speedmultiplier to the walkspeed
        }
        if (!sprint && !crouch)                     // if not sprinting or crouching
        {
            horizontalSpeed = walkSpeed;            // set the speedmultiplier to the walkspeed
        }
        if (!crouch && !IsCeiled())                 // if not crouching and not ceiled
        {
            // let the player uncrouch by changing the current playerscale to the usual scale using lerp
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, playerHeight, 1), currentCrouchLerp);
        }

        // if the player is not touching the ground
        if (!IsGrounded())
        {
            // increase the smoothtime in air for a smooth landing
            smoothInputSpeed = smoothInputSpeedAir;

            // lessen the horizontal speed when in air
            rb.velocity = new Vector3(rb.velocity.x * .999f, rb.velocity.y, rb.velocity.z *.999f);
        }
        // else, so if the player is touching the ground
        else
        {
            // changing the smoothtime back to the standard
            smoothInputSpeed = smoothInputSpeedGround;
        }

        // normalize the inputs and store them
        horizontalMove = new Vector3(inputX, 0, inputZ).normalized;

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

        // save the mouse movement in the X axis in mousePosX and multiply it by the velocity
        mousePosX += rotationX * sensitivity * .25f;
        // save the mouse movement in the Y axis in mousePosY and multiply it by the velocity
        mousePosY += rotationY * sensitivity * .25f;
        // clamp the vertical movement between -90 and 90 (otherwise the camera will turn around)
        mousePosY = Mathf.Clamp(mousePosY, -90, 90);

        // rotate the player with the saved mousemovement
        transform.rotation = Quaternion.Euler(0, mousePosX, 0);
        // rotate a gameobject in the player for the vertical camera movement
        camLookAt.transform.localRotation = Quaternion.Euler(mousePosY, 0, 0);
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
        if (Physics.Raycast(Ground, out groundData, cc.radius * 5f, isGround))
        {
            // calculate the angle  that the ground is from the player
            groundAngle = Vector3.Angle(groundData.normal, transform.up);
            // if the angle is lower than the set slopelimit, allow the player to be grounded
            if (groundAngle < maxSlopeLimit)
            {
                // calculate the distance the Ground ray should be poinding from the ground with the calculated angle, leaving a .01 margin (see ray Ground) using a trigonometric function
                groundDist = cc.radius / Mathf.Cos(Mathf.Deg2Rad * groundAngle) - cc.radius + .02f;
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

    // function called everytime IsCeiled() is mentioned in a script, a boolean value is returned
    public bool IsCeiled()
    {
        // calculate the current top of the Capsule Collider
        capsTop = transform.TransformPoint(cc.center + Vector3.up * cc.height / 2f);
        // create a ray that checks from the capsTop aiming up
        var Ceiling = new Ray(capsTop, transform.up);
        // store the raydata
        RaycastHit ceilingData;
        // if the ray hits something below the normal player height that is ground (the function should only be called when the player is crouched)
        if (Physics.Raycast(Ceiling, out ceilingData, cc.height - cc.height * playerCrouchHeight, isGround))
        {
            // return true so there should be a ceiling above the player
            return true;
        }
        // else so if the ray hits nothing
        else
        {
            // return true so there should be no ceiling above the player
            return false;
        }
    }
}
