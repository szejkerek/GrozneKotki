using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Settings")]
    public int maxEnemiesAlive;
    public int desiredEnemiesAlive = 40;
    public int initialEnemies = 40;

    public float startSpawnDelay = 2f;
    public float spawnDelayDecrease = 0.05f;
    public float minimumSpawnDelay = 0.3f;

    [Header("Deterministic setup")]
    public int seed = 12345;

    [Header("Spawn area")]
    public float spawnRadius = 3f;

    [Header("References")]
    public GameObject enemyPrefab;
    public GameObject fastEnemyPrefab;    // prefab szybkiego wroga
    List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

    [Header("Boss settings")]
    public GameObject firstBossPrefab;        // prefab bossa z komponentem Enemy
    public Transform firstBossSpawnPoint;     // gdzie ma się pojawić boss
    public int firstBossKillThreshold = 25;   // po tylu zabójstwach pojawia się boss

    public int fastEnemyInterval = 5;         // co tylu spawnów normalnych ma być szybki wróg
    public int fastEnemyStartKills = 3;      // po tylu zabójstwach dopiero zacznij

    private float currentDelay;
    private int killedCount;
    private float timer;
    private int activeEnemies;
    private bool firstBossSpawned;

    private int normalSpawnCounter;           // licznik spawnów zwykłych wrogów

    void Awake()
    {
        Instance = this;
        spawnPoints = GetComponentsInChildren<SpawnPoint>().ToList();
    }

    void Start()
    {
        Random.InitState(seed);
        currentDelay = startSpawnDelay;

        Enemy.OnEnemyKilled += EnemyKilled;
        SpawnInitialWave();
    }

    private void OnDestroy()
    {
        Enemy.OnEnemyKilled -= EnemyKilled;
    }

    void Update()
    {
        if (activeEnemies < desiredEnemiesAlive)
        {
            int toSpawn = desiredEnemiesAlive - activeEnemies;

            for (int i = 0; i < toSpawn; i++)
            {
                if (!TrySpawnEnemy())
                    break;
            }
        }

        timer += Time.deltaTime;
        if (timer >= currentDelay)
        {
            timer = 0f;
            TrySpawnEnemy();
        }
    }

    private void SpawnInitialWave()
    {
        int safety = 1000;

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

        int index = Random.Range(0, active.Count);
        SpawnPoint chosen = active[index];

        SpawnEnemyAtPoint(chosen);
        return true;
    }

    public void EnemyKilled()
    {
        killedCount++;

        // w momencie osiągnięcia progu resetujemy licznik
        if (killedCount == fastEnemyStartKills)
        {
            normalSpawnCounter = 0;
        }

        if (!firstBossSpawned && killedCount >= firstBossKillThreshold)
        {
            SpawnFirstBoss();
        }

        if (currentDelay > minimumSpawnDelay)
        {
            currentDelay -= spawnDelayDecrease;
            if (currentDelay < minimumSpawnDelay)
                currentDelay = minimumSpawnDelay;
        }
    }

    private void SpawnEnemyAtPoint(SpawnPoint chosen)
    {
        Vector3 spawnPos = chosen.transform.position;

        if (spawnRadius > 0f)
        {
            Vector2 offset2D = Random.insideUnitCircle * spawnRadius;
            spawnPos += new Vector3(offset2D.x, 0f, offset2D.y);
        }

        GameObject prefabToSpawn = enemyPrefab;

        // szybcy wrogowie dopiero po fastEnemyStartKills
        if (killedCount >= fastEnemyStartKills)
        {
            normalSpawnCounter++;

            if (fastEnemyPrefab != null &&
                fastEnemyInterval > 0 &&
                normalSpawnCounter % fastEnemyInterval == 0)
            {
                prefabToSpawn = fastEnemyPrefab;
            }
        }

        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        activeEnemies++;
    }

    private void SpawnFirstBoss()
    {
        if (firstBossSpawned)
            return;

        if (firstBossPrefab == null)
        {
            Debug.LogWarning("FirstBoss prefab is not assigned on EnemySpawner");
            return;
        }

        Vector3 spawnPos = Vector3.zero;

        if (firstBossSpawnPoint != null)
            spawnPos = firstBossSpawnPoint.position;

        GameObject bossObj = Instantiate(firstBossPrefab, spawnPos, Quaternion.identity);
        activeEnemies++;

        firstBossSpawned = true;
        Debug.Log("FirstBoss spawned");
    }

    public void EnemyDespawned()
    {
        activeEnemies = Mathf.Max(0, activeEnemies - 1);
    }
}
