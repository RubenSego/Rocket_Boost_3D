using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [System.Serializable]
    public struct PlayerSnapshot
    {
        public bool hasData;
        public string checkpointId;
        public Vector3 position;
        public Quaternion rotation;
        public int health;
        public int normalAmmo;
        public int eggs;
    }

    [Header("References")]
    [SerializeField] private GameObject playerObject;

    [Header("Min on Respawn")]
    [SerializeField] private int minHealthOnRespawn = 2;
    [SerializeField] private int minNormalAmmoOnRespawn = 5;
    [SerializeField] private int minEggsOnRespawn = 2;

    [Header("UI Muerte")]
    [SerializeField] private DeathPopupUI deathPopupUI;

    private PlayerSnapshot initialSnapshot;
    private PlayerSnapshot currentCheckpointSnapshot;
    private bool hasReachedCheckpoint = false;

    public bool HasReachedCheckpoint => hasReachedCheckpoint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (playerObject == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) playerObject = found;
        }

        if (playerObject != null)
        {
            initialSnapshot = CaptureSnapshot(
                playerObject,
                "START",
                playerObject.transform.position,
                playerObject.transform.rotation
            );
        }
    }

    public void SaveCheckpoint(Checkpoint checkpoint, GameObject player)
    {
        if (checkpoint == null) return;

        GameObject targetPlayer = player != null ? player : playerObject;
        if (targetPlayer == null) return;

        currentCheckpointSnapshot = CaptureSnapshot(
            targetPlayer,
            checkpoint.CheckpointId,
            checkpoint.RespawnPosition,
            checkpoint.RespawnRotation
        );
        hasReachedCheckpoint = true;

        string keyPrefix = $"CP_{SceneManager.GetActiveScene().name}";
        PlayerPrefs.SetString($"{keyPrefix}_Id", currentCheckpointSnapshot.checkpointId);
        PlayerPrefs.SetInt($"{keyPrefix}_Health", currentCheckpointSnapshot.health);
        PlayerPrefs.SetInt($"{keyPrefix}_Ammo", currentCheckpointSnapshot.normalAmmo);
        PlayerPrefs.SetInt($"{keyPrefix}_Eggs", currentCheckpointSnapshot.eggs);
        PlayerPrefs.Save();
    }

    private PlayerSnapshot CaptureSnapshot(GameObject player, string id, Vector3 pos, Quaternion rot)
    {
        PlayerHealth healthComp = player.GetComponent<PlayerHealth>();
        PlayerShooting shootingComp = player.GetComponent<PlayerShooting>();
        PlayerEggDropper eggComp = player.GetComponent<PlayerEggDropper>();

        return new PlayerSnapshot
        {
            hasData = true,
            checkpointId = id,
            position = pos,
            rotation = rot,
            health = healthComp != null ? healthComp.CurrentHealth : 3,
            normalAmmo = shootingComp != null ? shootingComp.CurrentAmmo : 10,
            eggs = eggComp != null ? eggComp.CurrentEggs : 5
        };

    }

    public void RespawnAtCheckpoint()
    {
        Time.timeScale = 1f;
        if (playerObject == null) return;

        PlayerSnapshot snapshotToLoad = hasReachedCheckpoint && currentCheckpointSnapshot.hasData
            ? currentCheckpointSnapshot
            : initialSnapshot;

        if (!snapshotToLoad.hasData) return;

        playerObject.SetActive(true);

        Rigidbody rb = playerObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = snapshotToLoad.position;
            rb.rotation = snapshotToLoad.rotation;
        }
        else
        {
            playerObject.transform.SetPositionAndRotation(snapshotToLoad.position, snapshotToLoad.rotation);
        }

        int restoredHealth = snapshotToLoad.health;
        int restoredAmmo = snapshotToLoad.normalAmmo;
        int restoredEggs = snapshotToLoad.eggs;

        PlayerHealth healthComp = playerObject.GetComponent<PlayerHealth>();
        if (healthComp != null)
        {
            healthComp.RespawnWithHealth(restoredHealth);
        }

        PlayerShooting shootingComp = playerObject.GetComponent<PlayerShooting>();
        if (shootingComp != null)
        {
            shootingComp.SetAmmo(restoredAmmo);
        }

        PlayerEggDropper eggComp = playerObject.GetComponent<PlayerEggDropper>();
        if (eggComp != null)
        {
            eggComp.SetEggs(restoredEggs);
        }

        CollisionManager collisionComp = playerObject.GetComponent<CollisionManager>();
        if (collisionComp != null)
        {
            collisionComp.ResetControllable();
        }

        PlayerMovement movementComp = playerObject.GetComponent<PlayerMovement>();
        if (movementComp != null)
        {
            movementComp.ResetMovementState();
            movementComp.enabled = true;
        }

        if (deathPopupUI != null)
        {
            deathPopupUI.HidePopup();
        }
        
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        string keyPrefix = $"CP_{SceneManager.GetActiveScene().name}";
        PlayerPrefs.DeleteKey($"{keyPrefix}_Id");
        PlayerPrefs.DeleteKey($"{keyPrefix}_Health");
        PlayerPrefs.DeleteKey($"{keyPrefix}_Ammo");
        PlayerPrefs.DeleteKey($"{keyPrefix}_Eggs");
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OnPlayerDied()
    {
        if (deathPopupUI != null)
        {
            deathPopupUI.ShowPopup(hasReachedCheckpoint);
        }
    }
}