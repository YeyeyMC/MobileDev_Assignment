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

    [SerializeField] private float scoreRadius = 1f;
    [SerializeField] private int rounds = 5;
    
    public static int Score { get; private set; }
    
    private Rigidbody rb;
    private Vector3 spawnPosition;
    private Vector3 smoothedAccel;
    private GameObject currentRound;
    private int circlesCollected;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        spawnPosition = transform.position;
        Score = 0;
        
        if (Accelerometer.current != null) InputSystem.EnableDevice(Accelerometer.current);
        
        
    }

    private void FixedUpdate()
    {
        ApplyTilt();
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
}
