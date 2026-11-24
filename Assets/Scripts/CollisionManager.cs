using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip explosionAudioClip;
    [SerializeField] private AudioClip landingAudioClip;

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem explosionParticleSystem;
    [SerializeField] private ParticleSystem landingParticleSystem;

    [Header("Level Complete UI")]
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private bool pauseGameOnComplete = true;

    AudioSource audioSource;
    PlayerMovement playerMovement;
    PlayerHealth playerHealth;

    bool isControllable = true; 
    void Awake()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }
    }
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
                CompleteLevel();
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
    private void BeginCollisionLock(AudioClip audioClip)
    {
        isControllable = false;
        AudioManager.Instance?.PlaySFX(audioClip);
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }
    public void ManageCollision(AudioClip audioClip, ParticleSystem particleSystem)
    {
        isControllable = false;
        AudioManager.Instance?.PlaySFX(audioClip);
        particleSystem.Play();
        playerMovement.enabled = false;
    }
    public void ManageCollision(AudioClip audioClip, GameObject particleSystem)
    {
        isControllable = false;
        AudioManager.Instance?.PlaySFX(audioClip);
        if (particleSystem != null)
    {
        GameObject spawnedParticles = Instantiate(particleSystem, transform.position, Quaternion.identity);
        ParticleSystem ps = spawnedParticles.GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
        }
    }
        playerMovement.enabled = false;
    }
    public void ResetControllable()
    {
        isControllable = true;
    }
    private void CompleteLevel()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        if (pauseGameOnComplete)
        {
            Time.timeScale = 0f;
        }
    }
}
