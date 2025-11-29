using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerControllerInputSystem))]
public class Player : MonoBehaviour
{
    [Header("Skills")]
    public List<PlayerSkill> skills = new();

    private InputMap inputActions;
    public Transform spawnPoint;

    void Awake()
    {
        inputActions = new InputMap();
    }

    void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.AttackStrong.performed += OnAttackStrong;
        inputActions.Player.AttackFast.performed += OnAttackFast;
    }

    void OnDisable()
    {
        inputActions.Player.AttackStrong.performed -= OnAttackStrong;
        inputActions.Player.AttackFast.performed -= OnAttackFast;

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

    void UseSkill(int index)
    {
        skills[index].TryUse();
    }
}