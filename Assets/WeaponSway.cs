using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("References")]
    public Move playerMovement;
    public Transform sprintRotationTarget;

    [Header("Idle Sway")]
    public bool enableIdleSway = true;
    public float idleSwayAmount = 0.008f;
    public float idleSwaySpeed = 1.8f;

    [Header("Mouse Sway (position)")]
    public float swayPosAmount = 0.05f;
    public float swayPosSmoothTime = 0.08f;
    public float swayPosMax = 0.06f;

    [Header("Mouse Sway (rotation)")]
    public float swayRotAmount = 3f;
    public float swayRotSmoothTime = 0.06f;

    [Header("Movement Bob")]
    public float bobAmount = 0.015f;
    public float bobSpeed = 8f;
    public float sprintBobAmount = 0.03f;
    public float sprintBobSpeed = 13f;

    [Header("Sprint Lower & Swing")]
    public Vector3 sprintPositionOffset = new Vector3(0f, -0.1f, 0.15f);
    public Vector3 sprintRotationOffset = new Vector3(40f, -40f, 0f);
    public float sprintSwingAmount = 0.24f;
    public float sprintSwingSpeed = 9f;
    public float sprintTransitionSmoothTime = 0.18f;

    [Header("Debug")]
    public bool previewSprintPose;

    [Header("Rotation Spring")]
    public float rotationSpringSpeed = 12f;

    // --- Private ---
    private Move moveScript;
    private Vector3 restPosition;
    private Quaternion restRotation;
    private Quaternion sprintRotationTargetRestRotation;
    private Vector2 swayPosVel;
    private Vector2 swayRotVel;
    private Vector3 posVel;
    private Vector2 currentSwayPos;   // persisted across frames
    private Vector2 currentSwayRot;   // persisted across frames
    private float idlePhase;
    private float bobPhase;
    private float sprintSwingPhase;

    void Start()
    {
        moveScript = playerMovement ?? GetComponentInParent<Move>();
        restPosition = transform.localPosition;
        restRotation = transform.localRotation;

        if (sprintRotationTarget != null && sprintRotationTarget != transform)
        {
            sprintRotationTargetRestRotation = sprintRotationTarget.localRotation;
        }
    }

    void Update()
    {
        float mx = Input.GetAxisRaw("Mouse X");
        float my = Input.GetAxisRaw("Mouse Y");
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool moving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;
        bool sprinting = previewSprintPose || (moveScript != null && moveScript.IsRunning());

        // Advance phases
        idlePhase += Time.deltaTime * idleSwaySpeed;
        bobPhase += Time.deltaTime * (sprinting ? sprintBobSpeed : bobSpeed);
        sprintSwingPhase += Time.deltaTime * sprintSwingSpeed * (sprinting ? 1 : 0);

        // =====================
        // 1. IDLE SWAY
        // =====================
        Vector3 idleOffset = Vector3.zero;
        if (enableIdleSway && !moving && !sprinting)
        {
            idleOffset = new Vector3(
                Mathf.Sin(idlePhase) * idleSwayAmount,
                Mathf.Cos(idlePhase * 0.7f) * idleSwayAmount * 0.5f,
                0
            );
        }

        // =====================
        // 2. MOUSE SWAY (position)
        // =====================
        Vector2 swayTarget = new Vector2(-mx * swayPosAmount, -my * swayPosAmount);
        swayTarget.x = Mathf.Clamp(swayTarget.x, -swayPosMax, swayPosMax);
        swayTarget.y = Mathf.Clamp(swayTarget.y, -swayPosMax, swayPosMax);

        currentSwayPos.x = Mathf.SmoothDamp(currentSwayPos.x, swayTarget.x, ref swayPosVel.x, swayPosSmoothTime);
        currentSwayPos.y = Mathf.SmoothDamp(currentSwayPos.y, swayTarget.y, ref swayPosVel.y, swayPosSmoothTime);

        // =====================
        // 3. MOUSE SWAY (rotation)
        // =====================
        Vector2 swayRotTarget = new Vector2(my * swayRotAmount, -mx * swayRotAmount);
        currentSwayRot.x = Mathf.SmoothDamp(currentSwayRot.x, swayRotTarget.x, ref swayRotVel.x, swayRotSmoothTime);
        currentSwayRot.y = Mathf.SmoothDamp(currentSwayRot.y, swayRotTarget.y, ref swayRotVel.y, swayRotSmoothTime);

        // =====================
        // 4. MOVEMENT BOB
        // =====================
        Vector3 bobOffset = Vector3.zero;
        if (moving && !sprinting)
        {
            float b = bobAmount;
            bobOffset = new Vector3(
                Mathf.Cos(bobPhase * 0.5f) * b,
                Mathf.Sin(bobPhase) * b,
                0
            );
        }

        // =====================
        // 5. SPRINT LOWER + SWING
        // =====================
        Vector3 sprintTargetPos = Vector3.zero;
        Vector3 sprintTargetRot = Vector3.zero;
        if (sprinting)
        {
            sprintTargetPos = sprintPositionOffset + new Vector3(Mathf.Sin(sprintSwingPhase) * sprintSwingAmount, 0, 0);
            sprintTargetRot = sprintRotationOffset;
        }

        // =====================
        // 6. COMBINE & APPLY
        // =====================
        Vector3 finalPos = restPosition + idleOffset + bobOffset + (Vector3)currentSwayPos + sprintTargetPos;
        transform.localPosition = Vector3.SmoothDamp(
            transform.localPosition, finalPos, ref posVel, sprintTransitionSmoothTime
        );

        Quaternion swayRotation = restRotation * Quaternion.Euler(currentSwayRot.x, currentSwayRot.y, 0);
        Quaternion sprintRotation = Quaternion.Euler(sprintTargetRot);
        bool hasSeparateSprintRotationTarget = sprintRotationTarget != null && sprintRotationTarget != transform;

        Quaternion finalRot = hasSeparateSprintRotationTarget
            ? swayRotation
            : swayRotation * sprintRotation;

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation, finalRot, Time.deltaTime * rotationSpringSpeed
        );

        if (hasSeparateSprintRotationTarget)
        {
            Quaternion finalSprintTargetRot = sprintRotationTargetRestRotation * sprintRotation;
            sprintRotationTarget.localRotation = Quaternion.Slerp(
                sprintRotationTarget.localRotation, finalSprintTargetRot, Time.deltaTime * rotationSpringSpeed
            );
        }
    }
}