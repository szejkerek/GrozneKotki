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

    void Awake()
    {
        inputActions = new InputMap();
        capsule = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;   // smoother visual motion

        animator = GetComponentInChildren<Animator>();
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);

        animator.SetBool("Running", input.magnitude > 0.1f);

        float mag = input.magnitude;
        if (mag > 1f) input /= mag;

        Vector3 moveDelta = input * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDelta);

        // Smooth rotation
        if (input.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(input, Vector3.up);

            // rotationSpeed is a factor 0..infinity, so we map it to a 0..1 lerp value per physics tick
            float t = rotationSpeed * Time.fixedDeltaTime;
            t = Mathf.Clamp01(t);

            Quaternion smoothRot = Quaternion.Slerp(rb.rotation, targetRot, t);
            rb.MoveRotation(smoothRot);
        }
    }
}