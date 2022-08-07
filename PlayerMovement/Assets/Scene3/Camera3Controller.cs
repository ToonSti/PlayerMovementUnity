using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera3Controller : MonoBehaviour
{
    [SerializeField] private LayerMask camMask;             // layers the camera raycast will check
    [SerializeField] private Transform Player;              // the player corresponding to the camera

    [SerializeField] private float maxCamDistance = 10f;    // distance between the player an the camera
    [SerializeField] private float camLerp = .2f;           // amount of smoothing for the camera
    [SerializeField] private float sensitivity = 70;        // sensitivity of the vertical movement

    private Vector3 rayDirection;                           // direction the camera raycast will point
    private float rayDistance;                              // length of the camera raycast

    private Vector3 desiredCamPos;                          // position the camera moves towards
    private Vector3 currentCamPos;                          // current position of the camera
    private Vector3 camDirection;                           // position of the camera from the player
    private float currentCamDistance;                       // current distance between the camera and player
    private float realCamDistance;                          // horizontal distance between the camera and player
    private float camHeight;                                // vertical height of the camera
    private float mousePosY;                                // the mouse position in the y axis


    // Update is called at the start
    void Start()
    {
        // set rayDistance to check until just behind the camera
        rayDistance = maxCamDistance + 1;
        // set currentCamDistance equal to maxCamDistance
        currentCamDistance = maxCamDistance;
        // set a starting value for the camheight
        mousePosY = 1.5f;
    }
   
    // Update is called once per frame
    void FixedUpdate()
    {
        // set rayDirection from player to the camera
        rayDirection = transform.position - Player.transform.position;
        // see the ray visualized in the Scene view
        Debug.DrawRay(Player.transform.position, rayDirection, Color.yellow, .1f);

        // create the ray from the player poining to the rayDireciton
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

        // if the mouse position in the y axis (mousePosY) is between the currentCamDistance with a .1 margin
        // currentCamDistance is used as a margin for the mousePosY, because otherwise the trigonometric function for the realCamDistance will not work
        if (mousePosY <= currentCamDistance - .1f && mousePosY >= -currentCamDistance + .1f)
        {
            // change the mousePosY by the vertical mouse input
            mousePosY += Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivity;
            // clamp the mousePosY between the currentCamDistance with a .1 margin
            mousePosY = Mathf.Clamp(mousePosY, -currentCamDistance + .1f, currentCamDistance - .1f);
        }
        // if the mousePosY was greater
        else if (mousePosY > currentCamDistance - .1f)
        {
            // change the mousePosY within the currentCamDistance margin
            mousePosY = currentCamDistance - .1f;
        }
        // if the mousePosY was less
        else if (mousePosY < currentCamDistance - .1f)
        {
            // change the mousePosY within the currentCamDistance margin
            mousePosY = -currentCamDistance + .1f;
        }

        // change the camHeight to the calculated mousePosY
        camHeight = mousePosY;
        // calculate the realCamDistance with a trigonometric function
        realCamDistance = currentCamDistance * Mathf.Cos(Mathf.Asin(camHeight / currentCamDistance));
        // the position that the camera should be from the player
        camDirection = new Vector3(0, camHeight, -realCamDistance);
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