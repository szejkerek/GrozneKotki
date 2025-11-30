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
        
        transform.SetParent(null);
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
        inputActions.Player.SkillFast.performed += OnSkillFast;
    }

    void OnDisable()
    {
        inputActions.Player.AttackStrong.performed -= OnAttackStrong;
        inputActions.Player.AttackFast.performed -= OnAttackFast;
        inputActions.Player.SkillStrong.performed -= OnSkillStrong;
        inputActions.Player.SkillFast.performed -= OnSkillFast;

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

    private void OnSkillFast(InputAction.CallbackContext ctx)
    {
        UseSkill(3);
    }


    void UseSkill(int index)
    {
        if (skills[index].TryUse())
        {
            ghostRecorder.RecordSkillUse(index);
        }
    }
}