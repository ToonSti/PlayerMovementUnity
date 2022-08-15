using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera3Controller2 : MonoBehaviour
{
    [SerializeField] private LayerMask camMask;             // layers the camera raycast will check
    [SerializeField] private Transform Player;              // the player corresponding to the camera
    [SerializeField] private Player3Controller controller;  // the player corresponding to the camera

    [SerializeField] private float maxCamDistance = 10f;    // distance between the player an the camera
    [SerializeField] private float camLerp = .2f;           // amount of smoothing for the camera

    private Vector3 rayDirection;                           // direction the camera raycast will point
    private float rayDistance;                              // length of the camera raycast

    private Vector3 desiredCamPos;                          // position the camera moves towards
    private Vector3 currentCamPos;                          // current position of the camera
    private Vector3 camDirection;                           // position of the camera from the player
    private float currentCamDistance;                       // current distance between the camera and player


    // Update is called at the start
    void Start()
    {
        // set rayDistance to check until just behind the camera
        rayDistance = maxCamDistance + 1;
        // set currentCamDistance equal to maxCamDistance
        currentCamDistance = maxCamDistance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // set rayDirection from player to the camera
        rayDirection = transform.position - Player.transform.position;
        // see the ray visualized in the Scene view
        Debug.DrawRay(Player.transform.position, rayDirection, Color.yellow, .1f);

        // create the ray from the player pointing to the rayDireciton
        var rayHit = new Ray(Player.transform.position, rayDirection);
        // save the raydata in hitData
        RaycastHit hitData;

        // if the ray hits something within the raydistance and selected in the Layermask
        if (Physics.Raycast(rayHit, out hitData, rayDistance, camMask))
        {
            // if the distance between the hit object is smaller than the maxCamDistance
            if (hitData.distance < maxCamDistance)
            {
                // set the currentCamDistance just in from of the object hit
                currentCamDistance = hitData.distance - .5f;
            }
            // else, so if the distance is larger than the maxCamDistance (the ray checks 1 position further than the maxCamDistance, so it is a possibility)
            else
            {
                // set the currentCamDistance to the maxCamDistance
                currentCamDistance = maxCamDistance;
            }
        }
        // else, so if the ray hits nothing
        else
        {
            // set the currentCamDistance to the maxCamDistance
            currentCamDistance = maxCamDistance;
        }

        // the position that the camera should be from the player
        camDirection = new Vector3(0, 0, -currentCamDistance);
        // set the rotations for the camera equal to the players
        Quaternion rotation = Quaternion.Euler(Player.transform.eulerAngles.x, Player.transform.eulerAngles.y, Player.transform.eulerAngles.z);
        // define the position the camera should move to
        desiredCamPos = Player.position + rotation * camDirection;
        // gradually move the camera to the desiredCamPos using lerp
        currentCamPos = Vector3.Lerp(transform.position, desiredCamPos, camLerp);
        // place the camera at the currentCamPos
        transform.position = currentCamPos;
        // let teh camera look at the player
        transform.LookAt(Player.transform);
    }
}
