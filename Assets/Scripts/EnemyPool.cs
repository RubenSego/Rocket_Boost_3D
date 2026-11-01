using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [Serializable]
    public class PoolItem
    {
        [Tooltip("Enemy Description)")]
        public string poolId;
        [Tooltip("Enemy Prefab")]
        public GameObject prefab;
        [Tooltip("Initial amount")]
        public int initialSize = 5;
    }

    [Header("Enemy Config")]
    [SerializeField] private List<PoolItem> enemyTypes = new List<PoolItem>();

    // Dictionary for save object list of each enemy type.
    private Dictionary<string, List<GameObject>> poolDictionary = new Dictionary<string, List<GameObject>>();
    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializePool();
    }

    private void InitializePool()
    {
        foreach (PoolItem item in enemyTypes)
        {
            if (item.prefab == null || string.IsNullOrEmpty(item.poolId)) continue;

            if (!poolDictionary.ContainsKey(item.poolId))
            {
                poolDictionary.Add(item.poolId, new List<GameObject>());
                prefabDictionary.Add(item.poolId, item.prefab);

                for (int i = 0; i < item.initialSize; i++)
                {
                    CreateEnemyInstance(item.poolId, item.prefab);
                }
            }
        }
    }

    private GameObject CreateEnemyInstance(string poolId, GameObject prefab)
    {
        GameObject newEnemy = Instantiate(prefab, transform);
        newEnemy.SetActive(false);
        poolDictionary[poolId].Add(newEnemy);
        return newEnemy;
    }

    /// <summary>
    /// Get an available enemy or creates a new one.
    /// </summary>
    public GameObject GetEnemy(string poolId, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        if (!poolDictionary.ContainsKey(poolId))
        {
            return null;
        }

        List<GameObject> list = poolDictionary[poolId];
        GameObject enemy = list.Find(e => e != null && !e.activeInHierarchy);

        if (enemy == null)
        {
            enemy = CreateEnemyInstance(poolId, prefabDictionary[poolId]);
        }

        enemy.transform.position = spawnPosition;
        enemy.transform.rotation = spawnRotation;
        enemy.SetActive(true);

        return enemy;
    }
}