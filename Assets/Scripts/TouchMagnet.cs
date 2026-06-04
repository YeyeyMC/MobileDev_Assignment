using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchMagnet : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform ball;
    [SerializeField] private Rigidbody ballRigidbody;
    [SerializeField] private GameObject magnetIndicatorPrefab;

    [Header("Magnet Settings")] 
    [SerializeField] private float magnetForce = 35f;
    [SerializeField] private float maxMagnetDistance = 8f;
    [SerializeField] private float maxCharge = 3f;
    [SerializeField] private float rechargeSpeed = 0.75f;
    [SerializeField] private LayerMask groundMask = ~0;

    private float currentCharge;
    private bool isMagnetActive;
    private Vector3 magnetWorldPosition;
    private GameObject magnetIndicator;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Start()
    {
        currentCharge = maxCharge;

        if (ball != null && ballRigidbody == null) ballRigidbody = ball.GetComponent<Rigidbody>();

        if (magnetIndicatorPrefab != null)
        {
            magnetIndicator = Instantiate(magnetIndicatorPrefab);
            magnetIndicator.SetActive(false);
        }

        UpdateMagnetUI();
    }

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying) return;

        ReadTouchInput();
        RechargeIfInactive();
        UpdateIndicator();
        UpdateMagnetUI();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying) return;

        ApplyMagnetForce();
    }

    private void ReadTouchInput()
    {
        isMagnetActive = false;

        if (Touch.activeTouches.Count == 0 || currentCharge <= 0f) return;

        var touch = Touch.activeTouches[0];
        Ray ray = mainCamera.ScreenPointToRay(touch.screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            magnetWorldPosition = hit.point;
            isMagnetActive = true;
        }
    }

    private void ApplyMagnetForce()
    {
        if (!isMagnetActive || ballRigidbody == null) return;

        Vector3 toMagnet = magnetWorldPosition - ballRigidbody.position;
        toMagnet.y = 0f;

        float distance = toMagnet.magnitude;
        if (distance <= 0.1f || distance > maxMagnetDistance) return;

        float distanceMultiplier = 1f - Mathf.Clamp01(distance / maxMagnetDistance);
        ballRigidbody.AddForce(toMagnet.normalized * (magnetForce * distanceMultiplier), ForceMode.Acceleration);

        currentCharge -= Time.fixedDeltaTime;
        currentCharge = Mathf.Max(0f, currentCharge);
    }

    private void RechargeIfInactive()
    {
        if (isMagnetActive) return;

        currentCharge += rechargeSpeed * Time.deltaTime;
        currentCharge = Mathf.Min(maxCharge, currentCharge);
    }

    private void UpdateIndicator()
    {
        if (magnetIndicator == null) return;

        magnetIndicator.SetActive(isMagnetActive);

        if (isMagnetActive) magnetIndicator.transform.position = magnetWorldPosition + Vector3.up * 0.05f;
    }

    private void UpdateMagnetUI()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetMagnetUI(isMagnetActive, currentCharge / maxCharge);
        }
    }
}