using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerInputSystem : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    
    [Header("Rotation")]
    public float rotationSpeed = 15f;

    [Header("Mouse Aiming")]
    public bool useMouseAim = true;
    public LayerMask groundLayer = ~0;
    public float raycastDistance = 1000f;
    
    [Header("Debug")]
    public bool showDebugGizmos = true;

    private InputMap inputActions;
    private Rigidbody rb;
    private Animator animator;
    private Camera mainCam;
    
    private Vector3 lastMouseWorldPos;
    private bool hasValidMouseTarget = false;

    void Awake()
    {
        inputActions = new InputMap();
        rb = GetComponent<Rigidbody>();

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        if (GameplayManager.Instance != null && GameplayManager.Instance.mainCamera != null)
        {
            mainCam = GameplayManager.Instance.mainCamera.GetComponent<Camera>();
        }
        else
        {
            mainCam = Camera.main;
        }

        if (mainCam == null)
        {
            Debug.LogError("No camera found! Mouse aiming won't work.");
        }
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void Update()
    {
        if (useMouseAim && mainCam != null)
        {
            UpdateMouseWorldPosition();
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void UpdateMouseWorldPosition()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Ray ray = mainCam.ScreenPointToRay(mouseScreenPos);

        // Try raycast to ground
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, groundLayer))
        {
            lastMouseWorldPos = hit.point;
            hasValidMouseTarget = true;
        }
        else
        {
            // Fallback: use a plane at player height
            Plane groundPlane = new Plane(Vector3.up, rb.position);
            if (groundPlane.Raycast(ray, out float enter))
            {
                lastMouseWorldPos = ray.GetPoint(enter);
                hasValidMouseTarget = true;
            }
        }
    }

 private void HandleMovement()
{
    Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();

    // For top-down camera, we need screen-space directions
    Vector3 camForward = Vector3.forward;
    Vector3 camRight = Vector3.right;

    if (mainCam != null)
    {
        // Get the camera's right vector
        camRight = mainCam.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        // Get perpendicular direction - SWAPPED ORDER to fix direction
        camForward = Vector3.Cross(camRight, Vector3.up);
    }

    // Calculate movement direction
    Vector3 moveDir = (camForward * moveInput.y + camRight * moveInput.x);
    
    float magnitude = moveDir.magnitude;
    if (magnitude > 1f)
    {
        moveDir /= magnitude;
    }

    // Update animator
    if (animator != null)
    {
        animator.SetBool("Running", magnitude > 0.1f);
    }

    // Move
    Vector3 moveDelta = moveDir * moveSpeed * Time.fixedDeltaTime;
    rb.MovePosition(rb.position + moveDelta);
}

private void HandleRotation()
{
    Vector3 lookDirection = Vector3.zero;

    if (useMouseAim && hasValidMouseTarget)
    {
        // Look at mouse position
        lookDirection = lastMouseWorldPos - rb.position;
        lookDirection.y = 0f;
    }
    else
    {
        // Fallback to right stick
        Vector2 lookInput = inputActions.Player.Look.ReadValue<Vector2>();
        
        if (lookInput.sqrMagnitude > 0.1f)
        {
            Vector3 camForward = Vector3.forward;
            Vector3 camRight = Vector3.right;
            
            if (mainCam != null)
            {
                camRight = mainCam.transform.right;
                camRight.y = 0f;
                camRight.Normalize();
                
                // SWAPPED ORDER here too
                camForward = Vector3.Cross(camRight, Vector3.up);
            }
            
            lookDirection = (camForward * lookInput.y + camRight * lookInput.x);
        }
    }

    // Rotate towards target direction
    if (lookDirection.sqrMagnitude > 0.01f)
    {
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        Quaternion newRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(newRotation);
    }
}

    void OnDrawGizmos()
    {
        if (!showDebugGizmos || !Application.isPlaying) return;

        if (hasValidMouseTarget)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, lastMouseWorldPos);
            
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(lastMouseWorldPos, 0.3f);
        }
    }
}