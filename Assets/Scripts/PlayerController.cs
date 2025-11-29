using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerInputSystem : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;

    [Header("Ground")]
    public LayerMask groundMask;
    public float groundCheckAbove = 2f;
    public float groundCheckExtra = 2f;

    [Header("Collision")]
    public LayerMask wallMask; // walls layer
    public float skinWidth = 0.05f; // small offset to prevent penetration

    private InputMap inputActions;
    private CapsuleCollider capsule;
    private Rigidbody rb;

    void Awake()
    {
        inputActions = new InputMap();
        capsule = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        rb.useGravity = true;
        rb.isKinematic = false; // dynamic for collisions
        rb.constraints = RigidbodyConstraints.FreezeRotation; // prevent physics rotation
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void FixedUpdate()
    {
        HandleMovement();
        SnapToGround();
    }

    private void HandleMovement()
    {
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);

        float mag = input.magnitude;
        if (mag > 1f) input /= mag;

        Vector3 moveDelta = input * moveSpeed * Time.fixedDeltaTime;

        // --- Custom collision handling for triggers/walls ---
        if (wallMask != 0)
        {
            Vector3 capsuleBottom = transform.position + capsule.center - Vector3.up * capsule.height * 0.5f;
            Vector3 capsuleTop = capsuleBottom + Vector3.up * capsule.height;

            Collider[] hits = Physics.OverlapCapsule(
                capsuleBottom + moveDelta,
                capsuleTop + moveDelta,
                capsule.radius,
                wallMask,
                QueryTriggerInteraction.Collide
            );

            foreach (Collider hit in hits)
            {
                Vector3 closest = hit.ClosestPoint(transform.position);
                Vector3 pushDir = (transform.position - closest).normalized;
                moveDelta += pushDir * skinWidth;
            }
        }

        rb.MovePosition(rb.position + moveDelta);

        // --- Rotation ---
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

    private void SnapToGround()
    {
        Bounds bounds = capsule.bounds;
        Vector3 origin = new Vector3(bounds.center.x, bounds.max.y + groundCheckAbove, bounds.center.z);
        float rayLength = groundCheckAbove + groundCheckExtra + bounds.extents.y;

        Debug.DrawRay(origin, Vector3.down * rayLength, Color.red);

        if (Physics.Raycast(
            origin,
            Vector3.down,
            out RaycastHit hit,
            rayLength,
            groundMask,
            QueryTriggerInteraction.Ignore
        ))
        {
            float bottomToGround = hit.point.y - bounds.min.y;
            Vector3 pos = rb.position;
            pos.y += bottomToGround;
            rb.MovePosition(pos);
        }
    }
}
