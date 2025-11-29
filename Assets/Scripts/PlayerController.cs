using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerInputSystem : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;

    private InputMap inputActions;
    private CapsuleCollider capsule;
    private Rigidbody rb;
    private Animator animator;

    void Awake()
    {
        inputActions = new InputMap();
        capsule = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        rb.useGravity = true;                 // Rigidbody gravity
        rb.isKinematic = false;               // dynamic rigidbody for collisions
        rb.constraints = RigidbodyConstraints.FreezeRotation;

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

        // Let Rigidbody and colliders handle all collisions
        rb.MovePosition(rb.position + moveDelta);

        // Rotation
        if (input.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(input, Vector3.up);
            rb.MoveRotation(Quaternion.RotateTowards(
                rb.rotation,
                targetRot,
                rotationSpeed * Time.fixedDeltaTime
            ));
        }
    }
}