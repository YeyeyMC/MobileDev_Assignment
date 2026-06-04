using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class TouchExample : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float moveSpeed = 15;

    private Vector3 targetPosition;
    private bool isDragging;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }
    
    void Start()
    {
        targetPosition = transform.position;
    }
    
    void Update()
    {
        HandleTouch();
        MoveToTarget();
    }

    private void HandleTouch()
    {
        if (Touch.activeTouches.Count == 0)
        {
            isDragging = false;
            return;
        }
        
        var touch = Touch.activeTouches[0];
        if (touch.phase == TouchPhase.Began)
        {
            targetPosition = ScreenToWorld(touch.screenPosition);
            isDragging = true;
        }
        else if (touch.phase == TouchPhase.Moved && isDragging)
        {
            targetPosition = ScreenToWorld(touch.screenPosition);
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            isDragging = false;
        }
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    private Vector3 ScreenToWorld(Vector2 screenPos)
    {
        float z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        return mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, z));
    }
}
