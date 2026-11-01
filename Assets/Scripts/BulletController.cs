using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float bulletLifeTime = 1f;
    [SerializeField] private int damage = 1;

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
            DeactivateBullet();
        }

        // TODO: Particles. 
        // TODO: If bullet hits enemies, reduce enemy health.
        // TODO: If bullet hits obstacles, destroy obstacles.
        // TODO: If bullet hits player, reduce player health.
    }
}
