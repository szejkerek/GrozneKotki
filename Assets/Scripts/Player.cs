using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerControllerInputSystem))]
public class Player : MonoBehaviour
{
    [Header("Skills")]
    public PlayerSkill primarySkill;

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
        if (primarySkill != null && inputActions.Player.Attack.triggered)
        {
            Debug.Log("X");
           // primarySkill.TryUse(this);
        }
    }
}