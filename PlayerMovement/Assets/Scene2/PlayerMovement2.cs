using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    public CharacterController controller;      // variable to controll the CharacterController script or use variables from it

    private bool jump = false;                  // variable to define if the player wants to jump
    private Vector3 input;                      // variable to store the horizontal inputs

    // Update is called once per frame
    void Update()
    {
        // get the horizontal inputs and put them in a vector 3
        input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        // if the player presses the defined Jump inputs
        if (Input.GetButtonDown("Jump"))
        {
            // the CharacterController gets permission to run the jumping code
            jump = true;
        }
        // if the player is not grounded, so it jumped or fell off
        if (!controller.IsGrounded())
        {
            // the CharacterController will stop the jumping code
            jump = false;
        }
    }

    // FixedUpdate has the frequency of the physics system
    void FixedUpdate()
    {
        // send the defined inputs to the controller so the controller can use them
        controller.Move(input, jump);
    }
}
