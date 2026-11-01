using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;

    [Header("Invulnerability Settings")]
    [SerializeField] private float invulnerabilityDuration = 1.0f;
    private bool isInvulnerable = false;
    Renderer[] playerRenderers;

    void Start()
    {
        currentHealth = maxHealth;
        // Getting renders for blinking during invulnerability.
        playerRenderers = GetComponentsInChildren<Renderer>();
    }

    public void TakeDamage(int damage)
    {
        // We ignore damage when is invulnerable or dead.
        if (isInvulnerable || currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log($"Vida restante: {currentHealth}");

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
        foreach (Renderer rend in playerRenderers)
        {
            if (rend != null) rend.enabled = visible;
        }
    }

    private void Die()
    {
        Debug.Log("El jugador ha muerto.");
        // Deactivate control.
        gameObject.SetActive(false);
        // TODO: SFX Damage when player gets enemy damage.
        // TODO: SFX Death when an enemy kills the player. Is only set when player explodes when he collides objects with Explosion tag.
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
}