using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float bulletLifeTime = 1f;

    void OnEnable()
    {
        Invoke(nameof(DeactivateBullet), bulletLifeTime);
    }

    /// <summary>
    /// Deactivates the bullet.
    /// </summary>
    private void DeactivateBullet()
    {
        gameObject.SetActive(false);
    }

    void OTriggerEnter(Collider other)
    {
        // Deactivates the bullet when it collides with another objects.
        DeactivateBullet();

        // TODO: Particles. 
        // TODO: If bullet hits enemies, reduce enemy health.
        // TODO: If bullet hits obstacles, destroy obstacles.
        // TODO: If bullet hits player, reduce player health.
    }
}
