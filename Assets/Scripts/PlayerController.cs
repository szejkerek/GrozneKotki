using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerInputSystem : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;   // treat this as responsiveness

    private InputMap inputActions;
    private CapsuleCollider capsule;
    private Rigidbody rb;
    private Animator animator;

    // reference to the gameplay camera
    private Transform camTransform;

    private float fireCooldown;

    void Awake()
    {
        inputActions = new InputMap();
        capsule = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        // try to get camera from GameplayManager
        if (GameplayManager.Instance != null && GameplayManager.Instance.mainCamera != null)
        {
            camTransform = GameplayManager.Instance.mainCamera.transform;
        }
        else
        {
            // fallback if something is not wired yet
            Camera cam = Camera.main;
            if (cam != null) camTransform = cam.transform;
        }
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void FixedUpdate()
    {
        HandleMovementAndAiming();
    }

    private void HandleMovementAndAiming()
    {
        // left stick movement input
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();

        // right stick aim input
        Vector2 lookInput = inputActions.Player.Look.ReadValue<Vector2>();

        // compute camera based forward and right on horizontal plane
        Vector3 camForward = Vector3.forward;
        Vector3 camRight   = Vector3.right;

        if (camTransform != null)
        {
            // start with camera forward
            camForward = camTransform.forward;

            // if camera is very top down, forward is almost vertical
            // in that case use camera.up as "screen up"
            if (Mathf.Abs(camForward.y) > 0.5f)
            {
                camForward = camTransform.up;
            }

            camForward.y = 0f;
            camForward.Normalize();

            camRight = camTransform.right;
            camRight.y = 0f;
            camRight.Normalize();
        }

        // movement direction in world space based on camera
        // up on stick means up on the screen
        Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;

        animator.SetBool("Running", moveDir.magnitude > 0.1f);

        float mag = moveDir.magnitude;
        if (mag > 1f) moveDir /= mag;

        Vector3 moveDelta = moveDir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDelta);

        // aim direction in world space based on camera
        Vector3 lookDir = camForward * lookInput.y + camRight * lookInput.x;

        // if there is input from right stick, face that
        // otherwise face movement direction
        Vector3 faceDir = lookDir.sqrMagnitude > 0.001f ? lookDir : moveDir;

        if (faceDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(faceDir, Vector3.up);

            float t = rotationSpeed * Time.fixedDeltaTime;
            t = Mathf.Clamp01(t);

            Quaternion smoothRot = Quaternion.Slerp(rb.rotation, targetRot, t);
            rb.MoveRotation(smoothRot);
        }
    }

}
