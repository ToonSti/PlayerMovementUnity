/*

// call in void update
// horizontal movement
Vector3 movement = new Vector3(x, 0, z);                    // make the movement variable
movement = Vector3.ClampMagnitude(movement, 1);             // make sure maximal speed is 1 to every side (normalize)
transform.Translate(movement * speed * Time.deltaTime);     // apply movement to object

// call in update
// vertical movement (jumping)
rb.AddForce(new Vector2(0f, jumpForce));                    // apply force up with an amount of force

float x = Input.GetAxis("Horizontal");                      // get the input corresponding to the x axis
float z = Input.GetAxis("Vertical");                        // get the input corresponding to the z axis


for the animation PlayerMove, I could have just sped up the PlayerIdle animation, but since you will probably never use the Idle animation sped up, I changed the animation anyway.
In case you would like to speed up your animation:
    https://docs.unity3d.com/ScriptReference/Animator-speed.html
I dont know if this is the best and correct way of speeding up. 
Since I think you speed up the entire animator with this, you will probably have to set the speed value back to the original when switching to PlayerIdle and before PlayerJump.
(the link is also in Sources under "Other:".)

*/