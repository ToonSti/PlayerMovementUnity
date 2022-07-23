/*

// call in void update
// horizontal movement
Vector3 movement = new Vector3(x, 0, z);                    // make the movement variable
movement = Vector3.ClampMagnitude(movement, 1);             // make sure maximal speed is 1 to every side
transform.Translate(movement * speed * Time.deltaTime);     // apply movement to object

// call in update
// vertical movement (jumping)
rb.AddForce(new Vector2(0f, jumpForce));                    // apply force up with an amount of force

float x = Input.GetAxis("Horizontal");                      // get the input corresponding to the x axis
float z = Input.GetAxis("Vertical");                        // get the input corresponding to the z axis

*/