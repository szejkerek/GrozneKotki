using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;


public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    public int MaxEnemy = 250;
    InputMap input;
    private bool reseted = false;
    [SerializeField] private int staringTime = 60;
    EnemySpawner Spawner;
    public MainCamera mainCamera;
    Player player;
    public TimeBar TimeBar;

    void Awake()
    {
        Debug.Log("ProjectContexts: " + FindObjectsOfType<ProjectContext>().Length);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        mainCamera = FindFirstObjectByType<MainCamera>();
        player = FindFirstObjectByType<Player>();
        input = new InputMap();
        mainCamera.Initialize(player.transform);
    }
    
    [Inject]
    public void Construct(EnemySpawner spawner)
    {
        Debug.Log("[GameplayManager] Construct, spawner = " + spawner);
        this.Spawner = spawner;
    }


    private void Start()
    {
        Bootstrap.Instance.GhostRunManager.RespawnAllGhosts();
        Spawner.maxEnemiesAlive = MaxEnemy;
        TimeBar.timeMax =  staringTime;
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