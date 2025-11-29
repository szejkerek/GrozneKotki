using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CapsuleCollider))]
public class PlayerControllerInputSystem : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;

    [Header("Ground")]
    public LayerMask groundMask;
    public float groundCheckAbove = 2f;
    public float groundCheckExtra = 2f;

    private InputMap inputActions;
    private CapsuleCollider capsule;

    void Awake()
    {
        inputActions = new InputMap();
        capsule = GetComponent<CapsuleCollider>();
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        // movement from input system
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);

        float mag = input.magnitude;
        if (mag > 1f)
            input /= mag;

        Vector3 moveDelta = input * moveSpeed * Time.deltaTime;
        transform.position += moveDelta;

        SnapToGround();

        if (input.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(input, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void SnapToGround()
    {
        if (capsule == null)
            return;

        Bounds bounds = capsule.bounds;

        // start ray above the top of the capsule
        Vector3 origin = new Vector3(
            bounds.center.x,
            bounds.max.y + groundCheckAbove,
            bounds.center.z
        );

        float rayLength = groundCheckAbove + groundCheckExtra + bounds.extents.y;

        // optional line so you can see the ray in the scene view
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
            // how far from current bottom to ground
            float bottomToGround = hit.point.y - bounds.min.y;

            Vector3 pos = transform.position;
            pos.y += bottomToGround;
            transform.position = pos;
        }
    }

}
