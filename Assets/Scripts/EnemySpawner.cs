using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnConfig
    {
        [Tooltip("Pool ID")]
        public string poolId = "SmallBird";
        [Tooltip("Spawn Point")]
        public Transform spawnPoint;
        [Tooltip("Spawn interval")]
        public float spawnInterval = 3.0f;
        [Tooltip("Max spawns")]
        public int maxSpawns = -1;
    }

    [Header("Waves config")]
    [SerializeField] private List<SpawnConfig> spawnConfigs = new List<SpawnConfig>();
    [SerializeField] private bool autoStart = true;

    [Header("Zone trigger")]
    [Tooltip("If is active, only spawns when the player comes in the area")]
    [SerializeField] private bool triggerOnPlayerEnter = false;

    private List<Coroutine> activeRoutines = new List<Coroutine>();
    private bool isSpawning = false;

    void Start()
    {
        if (autoStart && !triggerOnPlayerEnter)
        {
            StartSpawning();
        }
    }

    public void StartSpawning()
    {
        if (isSpawning) return;
        isSpawning = true;

        foreach (SpawnConfig config in spawnConfigs)
        {
            if (config.spawnPoint != null)
            {
                Coroutine routine = StartCoroutine(SpawnRoutine(config));
                activeRoutines.Add(routine);
            }
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        foreach (Coroutine routine in activeRoutines)
        {
            if (routine != null) StopCoroutine(routine);
        }
        activeRoutines.Clear();
    }

    private IEnumerator SpawnRoutine(SpawnConfig config)
    {
        int spawnedCount = 0;

        while (isSpawning && (config.maxSpawns == -1 || spawnedCount < config.maxSpawns))
        {
            yield return new WaitForSeconds(config.spawnInterval);

            if (EnemyPool.Instance != null)
            {
                EnemyPool.Instance.GetEnemy(
                    config.poolId, 
                    config.spawnPoint.position, 
                    config.spawnPoint.rotation
                );
                spawnedCount++;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnPlayerEnter && other.CompareTag("Player"))
        {
            StartSpawning();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerOnPlayerEnter && other.CompareTag("Player"))
        {
            StopSpawning();
        }
    }
}
