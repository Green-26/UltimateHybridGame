using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    
    [Header("Target Settings")]
    public Transform target;                    // Player or vehicle to follow
    public bool followPlayer = true;
    
    [Header("Follow Settings")]
    public Vector3 offset = new Vector3(0, 2, -5);
    public float followSpeed = 5f;
    public float rotationSpeed = 3f;
    public float smoothTime = 0.3f;
    
    [Header("Zoom Settings")]
    public float defaultZoom = 5f;
    public float aimZoom = 3f;
    public float vehicleZoom = 8f;
    public float zoomSpeed = 2f;
    private float currentZoom;
    private float targetZoom;
    
    [Header("Camera Shake")]
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.1f;
    private bool isShaking = false;
    
    [Header("Field of View")]
    public float defaultFOV = 60f;
    public float runFOV = 70f;
    public float vehicleFOV = 80f;
    public float nitrousFOV = 90f;
    public float fovSpeed = 5f;
    private float currentFOV;
    private float targetFOV;
    
    [Header("Camera Transitions")]
    public float transitionDuration = 0.5f;
    private bool isTransitioning = false;
    private Vector3 transitionStartPos;
    private Vector3 transitionTargetPos;
    private float transitionTimer = 0f;
    
    [Header("Camera Boundaries")]
    public Vector2 xBounds = new Vector2(-50f, 50f);
    public Vector2 zBounds = new Vector2(-50f, 50f);
    public bool useBounds = false;
    
    [Header("Collision Detection")]
    public LayerMask obstacleLayers = -1;
    public float collisionRadius = 0.3f;
    private Vector3 desiredPosition;
    
    [Header("Visual Effects")]
    public GameObject vignetteEffect;
    public GameObject speedLinesEffect;
    
    private Camera cam;
    private Vector3 velocity = Vector3.zero;
    private float currentSpeed = 0f;
    private bool isInVehicle = false;
    private bool isAiming = false;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;
        
        // Initialize values
        currentZoom = defaultZoom;
        targetZoom = defaultZoom;
        currentFOV = defaultFOV;
        targetFOV = defaultFOV;
        
        // Find player if not assigned
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
                target = player.transform;
        }
        
        // Set initial position
        if (target)
        {
            transform.position = target.position + offset;
            transform.LookAt(target);
        }
    }
    
    void LateUpdate()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }
        
        UpdateTarget();
        UpdateZoom();
        UpdateFOV();
        UpdateCameraMovement();
        HandleCollision();
        HandleVisualEffects();
    }
    
    void FindTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
            target = player.transform;
    }
    
    void UpdateTarget()
    {
        // Check if player is in vehicle
        VehicleController vehicle = FindObjectOfType<VehicleController>();
        if (vehicle && vehicle.IsPlayerInside())
        {
            if (!isInVehicle)
            {
                isInVehicle = true;
                StartTransition(vehicle.transform);
                targetZoom = vehicleZoom;
                targetFOV = vehicleFOV;
            }
            target = vehicle.transform;
            
            // Get vehicle speed for effects
            currentSpeed = vehicle.GetHealthPercent() > 0 ? 30f : 0f;
        }
        else
        {
            if (isInVehicle)
            {
                isInVehicle = false;
                StartTransition(GameObject.FindGameObjectWithTag("Player")?.transform);
                targetZoom = defaultZoom;
                targetFOV = defaultFOV;
            }
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
            
            // Get player speed
            PlayerController player = target?.GetComponent<PlayerController>();
            if (player)
            {
                currentSpeed = player.GetHealthPercent() > 0 ? 20f : 0f;
            }
        }
        
        // Check aiming
        if (!isInVehicle && Input.GetMouseButton(1))
        {
            if (!isAiming)
            {
                isAiming = true;
                targetZoom = aimZoom;
            }
        }
        else
        {
            if (isAiming)
            {
                isAiming = false;
                targetZoom = isInVehicle ? vehicleZoom : defaultZoom;
            }
        }
    }
    
    void UpdateZoom()
    {
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, zoomSpeed * Time.deltaTime);
    }
    
    void UpdateFOV()
    {
        // Dynamic FOV based on speed
        if (currentSpeed > 10f)
        {
            float speedPercent = Mathf.Clamp01(currentSpeed / (isInVehicle ? 80f : 30f));
            float fovTarget = targetFOV + (speedPercent * 10f);
            currentFOV = Mathf.Lerp(currentFOV, fovTarget, fovSpeed * Time.deltaTime);
        }
        else
        {
            currentFOV = Mathf.Lerp(currentFOV, targetFOV, fovSpeed * Time.deltaTime);
        }
        
        cam.fieldOfView = currentFOV;
    }
    
    void UpdateCameraMovement()
    {
        if (isTransitioning)
        {
            // Handle transition
            transitionTimer += Time.deltaTime;
            float t = transitionTimer / transitionDuration;
            t = Mathf.SmoothStep(0, 1, t);
            
            transform.position = Vector3.Lerp(transitionStartPos, transitionTargetPos, t);
            transform.LookAt(target);
            
            if (t >= 1f)
            {
                isTransitioning = false;
            }
        }
        else
        {
            // Normal follow
            desiredPosition = target.position + (transform.rotation * offset * currentZoom);
            
            // Apply bounds
            if (useBounds)
            {
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, xBounds.x, xBounds.y);
                desiredPosition.z = Mathf.Clamp(desiredPosition.z, zBounds.x, zBounds.y);
            }
            
            // Smooth follow
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
            transform.LookAt(target);
        }
    }
    
    void HandleCollision()
    {
        // Cast ray from target to camera position
        Vector3 direction = transform.position - target.position;
        float distance = direction.magnitude;
        direction.Normalize();
        
        RaycastHit hit;
        if (Physics.SphereCast(target.position, collisionRadius, direction, out hit, distance, obstacleLayers))
        {
            // Move camera closer if something blocks view
            Vector3 newPos = target.position + direction * (hit.distance - 0.2f);
            transform.position = newPos;
        }
    }
    
    void HandleVisualEffects()
    {
        // Speed lines effect
        if (speedLinesEffect)
        {
            float speedPercent = Mathf.Clamp01(currentSpeed / (isInVehicle ? 60f : 20f));
            speedLinesEffect.SetActive(speedPercent > 0.8f);
        }
        
        // Vignette effect on damage
        if (vignetteEffect)
        {
            // Will be controlled by damage events
        }
    }
    
    #region Public Methods
    
    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }
    
    IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float x = Random.Range(-magnitude, magnitude);
            float y = Random.Range(-magnitude, magnitude);
            transform.localPosition = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.localPosition = originalPos;
    }
    
    public void StartTransition(Transform newTarget)
    {
        if (newTarget == null) return;
        
        isTransitioning = true;
        transitionTimer = 0f;
        transitionStartPos = transform.position;
        transitionTargetPos = newTarget.position + offset;
        target = newTarget;
    }
    
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        StartTransition(newTarget);
    }
    
    public void SetZoom(float zoom, float duration = 0.5f)
    {
        targetZoom = zoom;
    }
    
    public void ResetZoom()
    {
        targetZoom = isInVehicle ? vehicleZoom : defaultZoom;
    }
    
    public void OnPlayerDamage()
    {
        ShakeCamera(0.2f, 0.15f);
        
        // FOV pulse effect
        StartCoroutine(FOVPulse());
    }
    
    IEnumerator FOVPulse()
    {
        float originalFOV = targetFOV;
        targetFOV = originalFOV + 5f;
        yield return new WaitForSeconds(0.1f);
        targetFOV = originalFOV;
    }
    
    public void OnVehicleBoost()
    {
        ShakeCamera(0.3f, 0.25f);
        StartCoroutine(FOVPulse());
    }
    
    public void OnExplosion(Vector3 position)
    {
        float distance = Vector3.Distance(transform.position, position);
        float shakeIntensity = Mathf.Clamp01(1f - (distance / 10f)) * 0.5f;
        ShakeCamera(0.3f, shakeIntensity);
    }
    
    #endregion
}