using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform playerHead;  // The neck part of the player
    [SerializeField]
    private Transform camera;      // Camera, if needed for other purposes (e.g., head follow)

    [SerializeField]
    private Transform crosshair;

    public float sensitivityX = 2f;  // Horizontal sensitivity for mouse movement
    public float minX = -90f;  // Minimum rotation for yaw (leftmost)
    public float maxX = 90f;   // Maximum rotation for yaw (rightmost)

    private float rotationX = 0f;  // Current rotation along the X-axis (yaw)

    // Start is called before the first frame update
    void Start()
    {
        // Optionally lock the cursor to the center of the screen for better control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        RotateHeadWithMouseInput();
    }

    private void RotateHeadWithMouseInput()
    {
        // Get the mouse movement input (horizontal only)
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX;

        // Update horizontal rotation (yaw) of the neck
        rotationX += mouseX;
        rotationX = Mathf.Clamp(rotationX, minX, maxX);  // Clamp horizontal rotation to a 180-degree range

        // Apply rotation to the neck
        playerHead.localEulerAngles = new Vector3(playerHead.localEulerAngles.x, rotationX, playerHead.localEulerAngles.z);
    }




    //TODO 

     /* 
      * We need vertical camera movement as well
      * Align camera position
      * adjust sensitivity
      * adsjust min max rotation degree
      * Move canvas and align with mouse cursor direction
      * 
      * 
      * 
      * 
      * 
      * */
}
