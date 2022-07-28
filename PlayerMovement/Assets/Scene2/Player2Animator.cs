using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Animator : MonoBehaviour
{
    public CharacterController controller;      // variable to controll the CharacterController script or use variables from it
    public Animator animator;                   // variable to controll the animator component

    // Update is called once per frame
    void Update()
    {
        // if the player is touching the ground
        if (controller.IsGrounded())
        {
            // set the animator parameter to false so change the animation to Idle or Move
            animator.SetBool("Jump", false);
        }
        // else, so if the player is not touching the ground
        else
        {
            // set the animator parameter to true so change the animation to Jump
            animator.SetBool("Jump", true);
        }

        // if there are no inputs
        if (controller.horizontalMove == Vector3.zero)
        {
            // set the animator parameter to false so change the animation to Idle
            animator.SetBool("Move", false);
        }
        // else, so if there are any inputs
        else
        {
            // set the animator parameter to true so change the animation to Move
            animator.SetBool("Move", true);
        }
    }
}
