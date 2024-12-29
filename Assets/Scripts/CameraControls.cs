using UnityEngine;

public class CameraControls : MonoBehaviour
{

    private void Update()
    {
        
        // ----------------------------Horizontal Movement----------------------------

        // Vector to store movement
        Vector3 inputDir = new Vector3(0, 0, 0);

        // Keyboard inputs
        if (Input.GetKey(KeyCode.W)) inputDir.z = +1f;
        if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
        if (Input.GetKey(KeyCode.A)) inputDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) inputDir.x = +1f;

        // Make sure W is always forward no matter the orientation
        Vector3 moveHorizontalDir = transform.forward * inputDir.z + transform.right * inputDir.x;

        float moveSpeedHorizontal = 10f;

        transform.position += moveHorizontalDir * moveSpeedHorizontal * Time.deltaTime;



        // Apply movement to camera


        // ----------------------------Vertical Movement----------------------------

        // New vector for up/down movement
        Vector3 moveVerticalDir = new Vector3(0, 0, 0);

        //Keyboard inputs
        if (Input.GetKey(KeyCode.LeftShift)) moveVerticalDir.y = -1f;
        if (Input.GetKey(KeyCode.Space)) moveVerticalDir.y = +1f;

        float moveSpeedVertical = 5f;

        // Uses world coordinates so up is always up no matter tilt
        transform.position += moveVerticalDir * moveSpeedVertical * Time.deltaTime;

        // ----------------------------Rotation----------------------------

        // Variables to store rotation
        float rotateHorizontalDir = 0f;
        float rotateVerticalDir = 0f;

        // Keyboard inputs
        if (Input.GetKey(KeyCode.Q)) rotateHorizontalDir = -1f;
        if (Input.GetKey(KeyCode.E)) rotateHorizontalDir = +1f;
        if (Input.GetKey(KeyCode.LeftArrow)) rotateHorizontalDir = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) rotateHorizontalDir = +1f;
        if (Input.GetKey(KeyCode.UpArrow)) rotateVerticalDir = -1f;
        if (Input.GetKey(KeyCode.DownArrow)) rotateVerticalDir = +1f;

        float rotateSpeed = 100f;

        // Apply rotation to camera
        transform.eulerAngles += new Vector3(rotateVerticalDir * rotateSpeed * Time.deltaTime, rotateHorizontalDir * rotateSpeed * Time.deltaTime, 0);
    }

}
