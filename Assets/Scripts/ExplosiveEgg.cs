using UnityEngine;

public class ExplosiveEgg : MonoBehaviour
{
    [Header("Explosion Config")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private int damage = 3;
    [SerializeField] private float maxLifeTime = 5f;
    [SerializeField] private LayerMask damageableLayers; 

    [Header("Effects")]
    [SerializeField] private GameObject explosionVFXPrefab;
    [SerializeField] private AudioClip explosionClip;

    private bool hasExploded = false;

    private void Start()
    {
        Destroy(gameObject, maxLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasExploded) return;

        if (other.CompareTag("Player") || other.transform.IsChildOf(transform.root)) return;

        Explode();
    }

    private void Explode()
    {
        hasExploded = true;

        if (explosionVFXPrefab != null)
        {
            GameObject effect = Instantiate(explosionVFXPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 1.5f);
        }

        if (explosionClip != null)
        {
            AudioManager.Instance?.PlaySFX(explosionClip);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player") || hit.transform.IsChildOf(transform.root)) continue;

            if (hit.CompareTag("Enemy"))
            {
                EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
            else if (hit.CompareTag("Explosive"))
            {
                ExplosiveObject explosive = hit.GetComponentInParent<ExplosiveObject>();
                if (explosive != null)
                {
                    explosive.Explode();
                }
            }
            else if (hit.CompareTag("Danger"))
            {
                Destroy(hit.gameObject);
            }
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
