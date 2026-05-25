using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Settings")]
    public float mouseSensitivity = 200f;

    [Header("References")]
    public Transform playerBody; 

    private float xRotation = 0f;

    void Start()
    {
        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Get mouse input, multiplied by sensitivity and Time.deltaTime to keep it framerate independent
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Debug.Log("Mouse Y value: " + Input.GetAxis("Mouse Y"));

        // Calculate the up/down rotation (pitch)
        xRotation -= mouseY;
        
        // Clamp it so you can't snap your neck looking too far up or down
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply the up/down rotation to the Camera (this object)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate the Player body left/right (yaw)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
