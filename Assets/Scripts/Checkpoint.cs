using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Identificador")]
    [SerializeField] private string checkpointId = "CP_01";

    [Header("Punto de Respawn")]
    [Tooltip("Opcional: si se deja vacío, se usa la posición de este objeto.")]
    [SerializeField] private Transform respawnPoint;

    [Header("Feedback Opcional")]
    [SerializeField] private AudioClip activateClip;
    [SerializeField] private ParticleSystem activateParticles;

    private bool activated = false;

    public string CheckpointId => checkpointId;
    public Vector3 RespawnPosition => respawnPoint != null ? respawnPoint.position : transform.position;
    public Quaternion RespawnRotation => respawnPoint != null ? respawnPoint.rotation : transform.rotation;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        Transform rootTransform = other.transform.root;
        GameObject playerObj = rootTransform.CompareTag("Player") ? rootTransform.gameObject : other.gameObject;

        if (!playerObj.CompareTag("Player")) return;

        activated = true;

        if (CheckpointManager.Instance != null)
        {
            CheckpointManager.Instance.SaveCheckpoint(this, playerObj);
        }

        if (activateClip != null)
        {
            AudioManager.Instance?.PlaySFX(activateClip);
        }

        if (activateParticles != null)
        {
            activateParticles.Play();
        }
    }
}