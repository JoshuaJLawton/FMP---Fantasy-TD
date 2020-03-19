using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Temporary Fix - Sets the Z rotation to 0 to stop the camera from rolling
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        // If the Right Mouse button is pressed
        if (Input.GetMouseButton(1))
        {
            // Sets the cursor to invisible and locks it to the centre of the screen
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            RotateCam();
            MoveCam();
        }
        // If the right mouse button is released
        else if (Input.GetMouseButtonUp(1))
        {
            // Resets the cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    // Gets the speed of the camera as a variable (Moves faster when left shift is held)
    float GetCamSpeed()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            return 40;
        }
        else
        {
            return 25;
        }
    }

    // Rotates the camera
    void RotateCam()
    {
        float Xrotation = Input.GetAxis("Mouse X") * 40 * Mathf.Deg2Rad;
        float Yrotation = Input.GetAxis("Mouse Y") * 40 * Mathf.Deg2Rad;

        transform.Rotate(Vector3.up, Xrotation);
        transform.Rotate(Vector3.right, -Yrotation);
    }

    // Moves the camera
    void MoveCam()
    {
        // W - Forwards / S - Backwards
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * (GetCamSpeed() * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * (GetCamSpeed() * Time.deltaTime);
        }

        // D - Right / A - Left
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * (GetCamSpeed() * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * (GetCamSpeed() * Time.deltaTime);
        }

        // E - Up / Q Down
        if (Input.GetKey(KeyCode.E))
        {
            transform.position += transform.up * (GetCamSpeed() * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            transform.position -= transform.up * (GetCamSpeed() * Time.deltaTime);
        }
    }
}


