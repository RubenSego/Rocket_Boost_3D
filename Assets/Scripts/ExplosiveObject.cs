using UnityEngine;

public class ExplosiveObject : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private AudioClip explosionSFX;

    [Header("Radius and damage")]
    [SerializeField] private float explosionRadius = 4f;
    [SerializeField] private int damage = 1;

    private bool hasExploded = false;

    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Instantiate explosion particles.
        if (explosionVFX != null)
        {
            GameObject vfx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            Destroy(vfx, 3f);
        }

        // Plays the sound effect.
        if (explosionSFX != null)
        {
            AudioSource.PlayClipAtPoint(explosionSFX, transform.position);
        }

        // Area damage.
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            // Damage to play if is close to the explosive object.
            if (hit.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hit.GetComponentInParent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
            }
            // Damage to the enemy if is close.
            else if (hit.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hit.GetComponentInParent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }
            }
        }

        // Destroy the object.
        Destroy(gameObject);
    }

    // When player collides with the explosive object, will explode.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.transform.IsChildOf(collision.transform.root))
        {
            // Opcional: si la colisión la causa el pájaro (o su jerarquía), se detona
            if (collision.gameObject.CompareTag("Player"))
            {
                Explode();
            }
        }
    }

    // Dibujar el radio en el editor para ajustarlo visualmente
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}