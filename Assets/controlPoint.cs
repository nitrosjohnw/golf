using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : MonoBehaviour
{

    /// Hello! Jeff
    public Rigidbody ball;
    public float rotationSpeed = 5f;
    public float distanceFromCenter = 5.0f;
    public float cameraHeight = 2.0f; // Adjust the camera height as needed
    public LayerMask collisionLayer;
    public float maxShootPower = 100f;

    private Vector3 offset;
    private float clickStartTime;
    private bool isCharging;

    void Start()
    {
        offset = transform.position - ball.position;
        isCharging = false;
    }

    void LateUpdate()
    {
        // Rotate the camera around the ball based on the mouse input.
        float horizontalInput = Input.GetAxis("Mouse X") * rotationSpeed;
        offset = Quaternion.AngleAxis(horizontalInput, Vector3.up) * offset;

        // Set the camera's position to be outside the ball but still follow the center.
        Vector3 desiredPosition = ball.position + offset.normalized * distanceFromCenter;
        desiredPosition.y += cameraHeight; // Raise the camera height

        // Perform a raycast to check for collisions on the Y-axis.
        Ray ray = new Ray(desiredPosition, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, collisionLayer))
        {
            // Adjust the camera position to the point where the ray hits an object.
            transform.position = new Vector3(desiredPosition.x, hit.point.y, desiredPosition.z);
        }
        else
        {
            // No collision, set the camera's position to the desired position.
            transform.position = desiredPosition;
        }

        // Make the camera look at the center of the ball.
        transform.LookAt(ball.position);

        if (Input.GetMouseButtonDown(0))
        {
            // Start charging when left-click is pressed
            isCharging = true;
            clickStartTime = Time.time;
        }

        if (isCharging)
        {
            float chargeDuration = Time.time - clickStartTime;
            float chargeRatio = Mathf.Clamp01(chargeDuration / 2f); // 2 seconds to full power
            float currentPower = maxShootPower * chargeRatio;

            if (Input.GetMouseButtonUp(0))
            {
                // Release left-click to shoot with the current power
                ball.velocity = transform.forward * currentPower;
                isCharging = false;
            }
        }
    }
}
