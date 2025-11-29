using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerControllerInputSystem))]
public class Player : MonoBehaviour
{
    [Header("Skills")]
    public PlayerSkill primarySkill;
    public PlayerSkill secondarySkill;

    InputMap inputActions;
    public Transform spawnPoint;

    void Awake()
    {
        inputActions = new InputMap();
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
        if (primarySkill != null && inputActions.Player.AttackFast.triggered)
        {
           primarySkill.TryUse();
        }
        
        if (secondarySkill != null && inputActions.Player.AttackStrong.triggered)
        {
            secondarySkill.TryUse();
        }
    }
}