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

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void FixedUpdate()
    {
        HandleMovementAndAiming();
    }

    private void HandleMovementAndAiming()
    {
        // lewa gałka ruch
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y);

        // animacja biegu po ruchu, nie po celowaniu
        animator.SetBool("Running", moveDir.magnitude > 0.1f);

        float mag = moveDir.magnitude;
        if (mag > 1f) moveDir /= mag;

        Vector3 moveDelta = moveDir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDelta);

        // prawa gałka celowanie
        Vector2 lookInput = inputActions.Player.Look.ReadValue<Vector2>();
        Vector3 lookDir = new Vector3(lookInput.x, 0f, lookInput.y);

        // jeśli jest wejście z prawej gałki, obracamy według niej
        // jeśli nie, obracamy według kierunku ruchu
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
