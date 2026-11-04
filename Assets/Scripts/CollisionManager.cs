using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip explosionAudioClip;
    [SerializeField] private AudioClip landingAudioClip;

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem explosionParticleSystem;
    [SerializeField] private ParticleSystem landingParticleSystem;
    AudioSource audioSource;
    PlayerMovement playerMovement;
    PlayerHealth playerHealth;

    bool isControllable = true; 
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if (!isControllable) return;
        switch (collision.gameObject.tag)
        {
            case "LaunchPad":
                Debug.Log("LaunchPad Collision Detected");
                break;
            case "LandingPad":
                ManageCollision(landingAudioClip, landingParticleSystem);
                break;
            case "Danger":
                // Apply damage.
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(1);
                }
                break;
            default:
                break;
        }
    }
    public void ManageCollision(AudioClip audioClip, ParticleSystem particleSystem)
    {
        isControllable = false;
        audioSource.Stop();
        audioSource.PlayOneShot(audioClip);
        particleSystem.Play();
        playerMovement.enabled = false;
    }
}
