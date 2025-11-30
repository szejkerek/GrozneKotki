using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    InputMap input;
    private bool reseted = false;
    [SerializeField] private int staringTime = 60;
    
    public MainCamera mainCamera;
    Player player;
    public TimeBar TimeBar;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        mainCamera = FindObjectOfType<MainCamera>();
        player = FindObjectOfType<Player>();
        input = new InputMap();
        mainCamera.Initialize(player.transform);
        TimeBar.timeMax =  staringTime;
    }

    private void Start()
    {
        Bootstrap.Instance.GhostRunManager.RespawnAllGhosts();
    }

    void OnEnable()
    {
        input.Enable();
        
        input.Player.Restart.performed += OnRestart;
        input.Player.MainMenu.performed += OnMainMenu;
        TimeBar.OnTimeDepleted += HandleLostTime;
    }

    private void HandleLostTime()
    {
        if(reseted)
            return;

        reseted = true;
        RestartLevel();
    }

    void OnDisable()
    {
        input.Disable();
        
        input.Player.Restart.performed -= OnRestart;
        input.Player.MainMenu.performed -= OnMainMenu;
        TimeBar.OnTimeDepleted -= HandleLostTime;
    }

    private void OnMainMenu(InputAction.CallbackContext obj)
    {
        ReturnToMenu();
    }

    private void OnRestart(InputAction.CallbackContext obj)
    {
        RestartLevel();
    }
    
    void Update()
    {
        if (input.Player.Restart.triggered)
        {
            RestartLevel();
        }
    }

    public void RestartLevel()
    {
        
        Bootstrap.Instance.SceneManager.ReloadCurrentScene();
        
    }

    public void ReturnToMenu(string menuSceneName = "MainMenu")
    {
        Time.timeScale = 1f;
        Bootstrap.Instance.SceneManager.LoadScene(menuSceneName);
        GhostRunManager.Instance.RemoveAllRuns();
    }
}