using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Data Profile")]
    [SerializeField] private EnemyData enemyData;

    [Header("Health Settings")]
    [Tooltip("Enemy Health")]
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private int currentHealth;

    [Header("Visual Effects")]
    [Tooltip("VFX Death")]
    [SerializeField] private GameObject explosionVfxPrefab;

    private bool isDead = false;
    private Collider enemyCollider;

    void Awake()
    {
        enemyCollider = GetComponent<Collider>();
        InitializeStats();
    }

    void OnEnable()
    {
        // Restore collider and death flag to reactivate the pool.
        if (enemyCollider != null) enemyCollider.enabled = true;
        isDead = false;
        // Restore stats.
        InitializeStats();
    }
    private void InitializeStats()
    {
        if (enemyData != null)
        {
            maxHealth = enemyData.maxHealth;
            explosionVfxPrefab = enemyData.explosionVfxPrefab;
        }
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (enemyCollider != null) enemyCollider.enabled = false;

        // Instance visual effect.
        if (explosionVfxPrefab != null)
        {
            GameObject explosionInstance = Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
            // Force to play the effect if play on awake is off.
            ParticleSystem ps = explosionInstance.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
            Destroy(explosionInstance, 2.0f);
        }

        // Deactivate for the pool.
        gameObject.SetActive(false);
    }
}
