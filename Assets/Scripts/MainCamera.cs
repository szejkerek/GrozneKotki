using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Position")]
    public float height = 12f;      // vertical distance above the player
    public float smoothTime = 0.2f; // position smoothing time
    public float maxSpeed = 40f;    // max follow speed

    private Vector3 velocity;

    public void Initialize(Transform target)
    {
        player = target;
        transform.SetParent(null);
        SnapToTarget();
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null)
            {
                Initialize(found.transform);
            }
        }
    }

    private void LateUpdate()
    {
        if (player == null) return;

        // position directly above the player
        Vector3 targetPos = new Vector3(
            player.position.x,
            player.position.y + height,
            player.position.z
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime,
            maxSpeed
        );
    }

    private void SnapToTarget()
    {
        if (player == null) return;

        Vector3 targetPos = new Vector3(
            player.position.x,
            player.position.y + height,
            player.position.z
        );

        transform.position = targetPos;
    }
}