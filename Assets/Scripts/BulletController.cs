using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float bulletLifeTime = 1f;
    [SerializeField] private int damage = 1;

    [Header("Visual Effects")]
    [SerializeField] private GameObject hitVFX;

    void OnEnable()
    {
        CancelInvoke();
        Invoke(nameof(DeactivateBullet), bulletLifeTime);
    }

    /// <summary>
    /// Deactivates the bullet.
    /// </summary>
    private void DeactivateBullet()
    {
        CancelInvoke();
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        // If collides with Player or children, ignore them.
        if (other.CompareTag("Player") || other.transform.IsChildOf(transform.root)) return;

        // If collides enemy, get the component EnemyHealth and apply damage.
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Deactivates the bullet when it collides with another objects.
            SpawnHitEffect();
            DeactivateBullet();
            return;
        }

        // If collides with explosive object, get the component ExplosiveObject and explodes.
        if (other.CompareTag("Explosive"))
        {
            ExplosiveObject explosive = other.GetComponentInParent<ExplosiveObject>();
            if (explosive != null)
            {
                explosive.Explode();
            }

            SpawnHitEffect();
            DeactivateBullet();
            return;
        }
        if (other.CompareTag("Danger"))
        {
            Destroy(other.gameObject); 

            SpawnHitEffect();
            DeactivateBullet();
            return;
        }
        // If collides with obstacles, walls or ground.
        SpawnHitEffect();
        DeactivateBullet();
    }

    private void SpawnHitEffect()
    {
        if (hitVFX != null)
        {
            Vector3 spawnPosition = transform.position - transform.forward * 0.2f; 
            GameObject effect = Instantiate(hitVFX, spawnPosition, Quaternion.identity);
            Destroy(effect, 1.5f);
        }
    }
}
