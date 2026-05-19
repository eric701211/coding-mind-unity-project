using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider),typeof(Rigidbody))]
public class Move : MonoBehaviour
{
    public float walkSpeed = 5;
    public float runSpeed = 10;
    public float backwardSpeed = 3f;
    public float strafeSpeed = 4f;
    public KeyCode runKey = KeyCode.LeftShift;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public bool IsRunning()
    {
        return Input.GetAxis("Vertical") > 0 && Input.GetKey(runKey);
    }

    void Update()
    {
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");

        float currentZSpeed = walkSpeed;
        if (verticalAxis > 0)
        {
            currentZSpeed = Input.GetKey(runKey) ? runSpeed : walkSpeed;
        }
        else if (verticalAxis < 0)
        {
            // COD style: cannot sprint backwards, walk slower backwards
            currentZSpeed = backwardSpeed;
        }

        // Strafe speed for horizontal
        float currentXSpeed = strafeSpeed;

        float inputX = horizontalAxis * currentXSpeed * Time.deltaTime;
        float inputZ = verticalAxis * currentZSpeed * Time.deltaTime;

        rb.transform.Translate(inputX, 0, inputZ);
    }
}
