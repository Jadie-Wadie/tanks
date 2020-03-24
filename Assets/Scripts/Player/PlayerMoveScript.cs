using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 12f;
    public float turnSpeed = 180f;

    [Header("Input")]
    private float movementInput;
    private float turnInput;

    [Header("Debug")]
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        movementInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
		// Forward and Backward
        Vector3 movement = transform.forward * movementInput * speed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);

		// Left and Right
        Quaternion turnRotation = Quaternion.Euler(0f, turnInput * turnSpeed * Time.deltaTime, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}