using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerControllerInputSystem))]
public class Player : MonoBehaviour
{
    [SerializeField] GhostRecorder ghostRecorder;
    [Header("Skills")]
    public List<PlayerSkill> skills = new();

    private InputMap inputActions;
    public Transform spawnPoint;

    // zakres losowej odchyłki pozycji
    [SerializeField] float randomOffsetRange = 0.5f;

    void Awake()
    {
        inputActions = new InputMap();
    }

    void Start()
    {
        if (spawnPoint != null)
        {
            Vector3 offset = new Vector3(
                Random.Range(-randomOffsetRange, randomOffsetRange),
                0f,
                Random.Range(-randomOffsetRange, randomOffsetRange)
            );

            transform.position = spawnPoint.position + offset;
        }
    }

    void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.AttackStrong.performed += OnAttackStrong;
        inputActions.Player.AttackFast.performed += OnAttackFast;
        inputActions.Player.SkillStrong.performed += OnSkillStrong;
    }

    void OnDisable()
    {
        inputActions.Player.AttackStrong.performed -= OnAttackStrong;
        inputActions.Player.AttackFast.performed -= OnAttackFast;
        inputActions.Player.SkillStrong.performed -= OnSkillStrong;

        inputActions.Disable();
    }

    private void OnAttackStrong(InputAction.CallbackContext ctx)
    {
        UseSkill(0);
    }

    private void OnAttackFast(InputAction.CallbackContext ctx)
    {
        UseSkill(1);
    }

    private void OnSkillStrong(InputAction.CallbackContext ctx)
    {
        UseSkill(2);
    }

    void UseSkill(int index)
    {
        skills[index].TryUse();
        ghostRecorder.RecordSkillUse(index);
    }
}