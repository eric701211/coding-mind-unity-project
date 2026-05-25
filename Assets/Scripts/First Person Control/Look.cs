using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Look : MonoBehaviour
{
    public float sensitivity = 1;
    public float smoothing = 2;

    public Transform charCamera;
    private Vector2 currentMouseLook;
    private Vector2 appliedMouseDelta;

    [Header("Head Bobbing")]
    public bool enableHeadBob = true;
    public float bobFrequency = 10f;
    public float bobAmount = 0.05f;
    public float sprintBobFrequency = 15f;
    public float sprintBobAmount = 0.1f;
    
    [Header("Bob Smoothing")]
    public float bobSmoothness = 10f;

    private float bobTimer = 0f;
    private Vector3 defaultCameraPos;
    private float currentBobAmp = 0f;
    private float currentBobFreq = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        charCamera = Camera.main.transform;
        defaultCameraPos = charCamera.localPosition;
    }

    void Update()
    {
        Vector2 smoothMouseDelta = Vector2.Scale(new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")), Vector2.one * sensitivity * smoothing);
        appliedMouseDelta = Vector2.Lerp(appliedMouseDelta, smoothMouseDelta, 1 / smoothing);
        currentMouseLook += appliedMouseDelta;
        currentMouseLook.y = Mathf.Clamp(currentMouseLook.y, -90, 90);

        charCamera.localRotation = Quaternion.AngleAxis(-currentMouseLook.y, Vector3.right);
        transform.localRotation = Quaternion.AngleAxis(currentMouseLook.x, Vector3.up);

        if (enableHeadBob)
        {
            // Use smoothed input for better velocity scaling
            float inputX = Input.GetAxis("Horizontal");
            float inputZ = Input.GetAxis("Vertical");
            float speedMultiplier = new Vector2(inputX, inputZ).magnitude;
            speedMultiplier = Mathf.Clamp01(speedMultiplier);

            bool isSprinting = Input.GetKey(KeyCode.LeftShift) && inputZ > 0;
            float targetFreq = isSprinting ? sprintBobFrequency : bobFrequency;
            float targetAmp = isSprinting ? sprintBobAmount : bobAmount;

            // Smoothly transition amplitude and frequency
            currentBobAmp = Mathf.Lerp(currentBobAmp, targetAmp * speedMultiplier, Time.deltaTime * bobSmoothness);
            currentBobFreq = Mathf.Lerp(currentBobFreq, targetFreq, Time.deltaTime * bobSmoothness);

            if (speedMultiplier > 0.1f)
            {
                bobTimer += Time.deltaTime * currentBobFreq;
            }

            // Calculate bob offsets (Figure-8 movement)
            float bobX = Mathf.Cos(bobTimer / 2f) * currentBobAmp;
            float bobY = Mathf.Sin(bobTimer) * currentBobAmp;

            charCamera.localPosition = new Vector3(
                defaultCameraPos.x + bobX, 
                defaultCameraPos.y + bobY, 
                defaultCameraPos.z
            );
        }
    }

    public void ApplyRecoil(float recoilAmount)
    {
        currentMouseLook.y += recoilAmount;
    }
}
