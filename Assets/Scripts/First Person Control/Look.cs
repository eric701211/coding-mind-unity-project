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

    private float bobTimer = 0f;
    private Vector3 defaultCameraPos;

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
            float inputX = Input.GetAxisRaw("Horizontal");
            float inputZ = Input.GetAxisRaw("Vertical");
            bool isMoving = Mathf.Abs(inputX) > 0.1f || Mathf.Abs(inputZ) > 0.1f;

            if (isMoving)
            {
                bool isSprinting = Input.GetKey(KeyCode.LeftShift);
                float freq = isSprinting ? sprintBobFrequency : bobFrequency;
                float amp = isSprinting ? sprintBobAmount : bobAmount;

                bobTimer += Time.deltaTime * freq;
                charCamera.localPosition = new Vector3(
                    defaultCameraPos.x + Mathf.Cos(bobTimer / 2f) * amp, 
                    defaultCameraPos.y + Mathf.Sin(bobTimer) * amp, 
                    defaultCameraPos.z
                );
            }
            else
            {
                bobTimer = 0f;
                charCamera.localPosition = Vector3.Lerp(charCamera.localPosition, defaultCameraPos, Time.deltaTime * 10f);
            }
        }
    }

    public void ApplyRecoil(float recoilAmount)
    {
        currentMouseLook.y += recoilAmount;
    }
}
