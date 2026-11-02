using UnityEngine;

public enum PowerUpType
{
    Health,
    Ammo
}

public class PowerUp : MonoBehaviour
{
    [Header("Type/Amount")]
    [SerializeField] private PowerUpType powerUpType;
    [SerializeField] private int amount = 1;

    [Header("SFX/VFX")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private GameObject pickupEffect;

    [Header("Animation")]
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float bobbingSpeed = 2f;
    [SerializeField] private float bobbingHeight = 0.25f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Continuous rotation about its Y axis.
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Vertical motion (up and down).
        float newY = startPos.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger detectó a: {other.gameObject.name} con Tag: '{other.gameObject.tag}'");
        // Checks if the player collides.
        if (other.CompareTag("Player"))
        {
            ApplyPowerUp(other.gameObject);
        }
    }

    private void ApplyPowerUp(GameObject player)
    {
        bool consumed = false;

        if (powerUpType == PowerUpType.Health)
        {
            // Busca PlayerHealth en el objeto colisionado, en sus padres o en sus hijos
            PlayerHealth health = player.GetComponentInParent<PlayerHealth>();
            if (health == null) health = player.GetComponentInChildren<PlayerHealth>();

            if (health != null)
            {
                Debug.Log("PlayerHealth encontrado con éxito. Intentando curar...");
                health.Heal(amount);
                consumed = true;
            }
            else
            {
                Debug.LogError($"Se detectó tag 'Player' en {player.name}, pero NO se encontró el script PlayerHealth en él ni en su jerarquía.");
            }
        }
        else if (powerUpType == PowerUpType.Ammo)
        {
            PlayerShooting shooting = player.GetComponentInParent<PlayerShooting>();
            if (shooting == null) shooting = player.GetComponentInChildren<PlayerShooting>();

            if (shooting != null)
            {
                shooting.AddAmmo(amount);
                consumed = true;
            }
            else
            {
                Debug.LogError($"No se encontró PlayerShooting en la jerarquía de {player.name}.");
            }
        }

        if (consumed)
        {
            Debug.Log("Power-up consumido y destruido.");

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}