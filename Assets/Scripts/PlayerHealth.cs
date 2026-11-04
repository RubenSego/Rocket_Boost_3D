using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private HealthHUD healthHUD;

    [Header("Visual Model")]
    [Tooltip("Sparrow Sprite")]
    [SerializeField] private GameObject sparrowModel;
    
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;

    [Header("Invulnerability Settings")]
    [SerializeField] private float invulnerabilityDuration = 1.0f;

    [Header("VFX/SFX Death")]
    [SerializeField] private AudioClip explosionAudioClip;
    [SerializeField] private ParticleSystem explosionParticleSystem;
    private bool isInvulnerable = false;
    Renderer[] playerRenderers;
    CollisionManager collisionManager;

    void Start()
    {
        currentHealth = maxHealth;
        // Getting renders for blinking during invulnerability.
        playerRenderers = GetComponentsInChildren<Renderer>();

        // Initialize the HUD.
        if (healthHUD != null)
        {
            healthHUD.SetupHUD(maxHealth);
        }
        collisionManager = GetComponent<CollisionManager>();
    }

    public void TakeDamage(int damage)
    {
        // We ignore damage when is invulnerable or dead.
        if (isInvulnerable || currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log($"Vida restante: {currentHealth}");

        // Update the HUD to break the egg.
        if (healthHUD != null)
        {
            healthHUD.UpdateHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityRoutine());
        }
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;

        // Blinks when is invulnerable.
        float elapsed = 0f;
        float blinkInterval = 0.1f;

        while (elapsed < invulnerabilityDuration)
        {
            SetRenderersVisible(false);
            yield return new WaitForSeconds(blinkInterval);
            SetRenderersVisible(true);
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval * 2;
        }

        // We set visible and vulnerable status.
        SetRenderersVisible(true);
        isInvulnerable = false;
    }

    private void SetRenderersVisible(bool visible)
    {
        if (sparrowModel != null)
        {
            sparrowModel.SetActive(visible);
        }
    }

    private void Die()
    {
        Debug.Log("El jugador ha muerto.");
        
        // Update the HUD to break the eggs.
        if (healthHUD != null)
        {
            healthHUD.UpdateHealth(0);
        }
        // Hide the visual model.
        SetRenderersVisible(false);

        // Deactivate collisions.
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Freeze the physics to avoid infinite falling.
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero; // En Unity 2023+ (o rb.velocity = Vector3.zero en versiones anteriores)
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        // Waiting before shutdown the GameObject.
        StartCoroutine(DieRoutine());
        collisionManager.ManageCollision(explosionAudioClip, explosionParticleSystem);
    }

    private IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(1.5f); // Tiempo suficiente para que termine el audio del choque
        gameObject.SetActive(false);
    }

    // Trigger enemy collider.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
    {
        // We get the component EnemyDamage of impacted object. 
        if (other.TryGetComponent(out EnemyDamage enemy))
        {
            TakeDamage(enemy.DamageAmount);
        }
        else
        {
            // Default damage.
            TakeDamage(1);
        }
    }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    public void Heal(int amount)
    {
        // Don't heal if player is dead.
        if (currentHealth <= 0) return;

        // Increase health without exceeding maxHealth.
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Vida recuperada. Actual: {currentHealth}");

        // Updates health bar.
        if (healthHUD != null)
        {
            healthHUD.UpdateHealth(currentHealth);
        }
    }
}