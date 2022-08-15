using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3Movement : MonoBehaviour
{
    public Player3Controller controller;        // variable to control the CharacterController script or use variables from it
    public Animator animator;                   // variable to control the Animator

    private float inputX;                       // variable to store the horizontal inputs
    private float inputZ;                       // variable to store the horizontal inputs
    private bool jump = false;                  // variable to define if the player wants to jump
    private bool crouch;                        // variable to define if the player wants to crouch
    private bool sprint;                        // variable to define if the player wants to sprint
    private float mousePosX;                    // variable to save mouse movement on the X axis
    private float mousePosY;                    // variable to save mouse movement on the Y axis

    // Update is called once per frame
    void Update()
    {
        // get the horizontal inputs and store them in a variable
        inputX = Input.GetAxisRaw("Horizontal");
        inputZ = Input.GetAxisRaw("Vertical");

        // get mouse movement on the X and Y axis and save them
        mousePosX = Input.GetAxisRaw("Mouse X");
        mousePosY = Input.GetAxisRaw("Mouse Y");

        // if the player presses the defined Jump inputs
        if (Input.GetButtonDown("Jump"))
        {
            // the CharacterController gets permission to run the jumping code
            jump = true;
        }
        // if the player is not grounded, so it jumped or fell off
        if (Input.GetButtonUp("Jump"))
        {
            // the CharacterController will stop the jumping code
            jump = false;
        }

        if (Input.GetButtonDown("Crouch"))              // if the crouch button is pressed
        {
            crouch = true;                              // save the input for the CharacterController and Animator
        }
        else if (Input.GetButtonUp("Crouch"))           // if the crouch button is released
        {
            crouch = false;                             // save the input for the CharacterController and Animator
        }

        if(Input.GetButtonDown("Sprint"))               // if the sprint button is pressed
        {
            sprint = true;                              // save the input for the CharacterController and Animator
        }
        else if (Input.GetButtonUp("Sprint"))           // if the sprint button is released
        {
            sprint = false;                             // save the input for the CharacterController and Animator
        }

        animator.SetBool("Jumping", jump);              // set the animator parameter to the just defined booleans
        animator.SetBool("Moving", sprint);             // set the animator parameter to the just defined booleans
    }

    // FixedUpdate has the frequency of the physics system
    void FixedUpdate()
    {
        // send the defined inputs to the controller so the controller can use them
        controller.Move(inputX, inputZ, jump, sprint, crouch, mousePosX, mousePosY);
    }
}
