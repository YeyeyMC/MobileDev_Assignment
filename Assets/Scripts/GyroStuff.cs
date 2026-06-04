using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GyroStuff : MonoBehaviour
{
    [SerializeField] private float tiltForce = 20f;
    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private float smoothing = 0.15f;
    [SerializeField] private bool invertX;
    [SerializeField] private bool invertY;
    
    private Rigidbody rb;
    private Vector3 smoothedAccel;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (Accelerometer.current != null) InputSystem.EnableDevice(Accelerometer.current);
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying) return;
        
        ApplyTilt();
        ClampVelocity();
    }

    private void ApplyTilt()
    {
        if (Accelerometer.current == null) return;

        Vector3 raw = Accelerometer.current.acceleration.ReadValue();
        smoothedAccel = Vector3.Lerp(smoothedAccel, raw, smoothing);
        
        float x = invertX ? smoothedAccel.x : -smoothedAccel.x;
        float z = invertY ? smoothedAccel.y : -smoothedAccel.y;
        
        rb.AddForce(new Vector3(x, 0, z) * tiltForce, ForceMode.Acceleration);
    }
    
    private void ClampVelocity()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
        }
    }
    
    public void ResetBall(Vector3 position)
    {
        transform.position = position;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
