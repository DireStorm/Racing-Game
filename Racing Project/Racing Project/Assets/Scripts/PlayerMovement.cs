using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //References
    public Rigidbody rb;

    public float forwardForce = 2000;

    private void FixedUpdate()
    {
        //rb.AddForce(0, 0, forwardForce*Time.deltaTime);

        // Keys for movement
        if (Input.GetKey("d"))
        {
            rb.AddForce(500 * Time.deltaTime, 0, 0);
        }

        if(Input.GetKey("a"))
        {
            rb.AddForce(-500 * Time.deltaTime, 0, 0);
        }

        if (Input.GetKey("w"))
        {
            rb.AddForce(0, 0, 500 * Time.deltaTime);
        }

        if (Input.GetKey("s"))
        {
            rb.AddForce(0, 0, -500 * Time.deltaTime);
        }
    }
}
