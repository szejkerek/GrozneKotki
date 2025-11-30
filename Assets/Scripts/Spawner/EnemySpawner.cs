using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Settings")]
    public int maxEnemiesAlive = 200;
    public int desiredEnemiesAlive = 40;      // ile wrogów chcemy mieć mniej więcej na scenie
    public int initialEnemies = 40;           // ilu wystawić od razu na starcie

    public float startSpawnDelay = 2f;
    public float spawnDelayDecrease = 0.05f;
    public float minimumSpawnDelay = 0.3f;

    [Header("Deterministic setup")]
    public int seed = 12345;

    [Header("Spawn area")]
    public float spawnRadius = 3f;            // promień losowego odchylenia od punktu spawnu

    [Header("References")]
    public GameObject enemyPrefab;
    List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    private float currentDelay;
    private int killedCount;
    private float timer;
    private int activeEnemies;

    void Awake()
    {
        Instance = this;
        spawnPoints = GetComponentsInChildren<SpawnPoint>().ToList();
    }

    void Start()
    {
        // stały seed dla deterministyczności
        Random.InitState(seed);
        currentDelay = startSpawnDelay;

        // pierwsza fala na starcie
        SpawnInitialWave();
    }

    void Update()
    {
        // pilnujemy aby było przynajmniej desiredEnemiesAlive przeciwników
        if (activeEnemies < desiredEnemiesAlive)
        {
            int toSpawn = desiredEnemiesAlive - activeEnemies;

            for (int i = 0; i < toSpawn; i++)
            {
                if (!TrySpawnEnemy())
                    break;
            }
        }

        // normalny ciągły spawn w czasie
        timer += Time.deltaTime;
        if (timer >= currentDelay)
        {
            timer = 0f;
            TrySpawnEnemy();
        }
    }

    private void SpawnInitialWave()
    {
        int safety = 1000; // zabezpieczenie przed nieskończoną pętlą

        int toSpawn = Mathf.Min(initialEnemies, maxEnemiesAlive);

        for (int i = 0; i < toSpawn && safety > 0; i++)
        {
            if (!TrySpawnEnemy())
                break;

            safety--;
        }
    }

    private bool TrySpawnEnemy()
    {
        if (enemyPrefab == null)
            return false;

        // limit jednocześnie żyjących
        if (activeEnemies >= maxEnemiesAlive)
            return false;

        List<SpawnPoint> active = new List<SpawnPoint>();
        foreach (var point in spawnPoints)
        {
            if (killedCount >= point.requiredKills)
                active.Add(point);
        }

        if (active.Count == 0)
            return false;

        // deterministyczny wybór punktu
        int index = Random.Range(0, active.Count);
        SpawnPoint chosen = active[index];

        SpawnEnemyAtPoint(chosen);
        return true;
    }

    private void SpawnEnemyAtPoint(SpawnPoint chosen)
    {
        Vector3 spawnPos = chosen.transform.position;

        if (spawnRadius > 0f)
        {
            // losowy offset w płaszczyźnie XZ
            Vector2 offset2D = Random.insideUnitCircle * spawnRadius;
            spawnPos += new Vector3(offset2D.x, 0f, offset2D.y);
        }

        GameObject obj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        activeEnemies++;
    }

    public void EnemyKilled()
    {
        killedCount++;
        
        if (currentDelay > minimumSpawnDelay)
        {
            currentDelay -= spawnDelayDecrease;
            if (currentDelay < minimumSpawnDelay)
                currentDelay = minimumSpawnDelay;
        }
    }
}
