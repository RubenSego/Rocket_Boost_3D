using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEggDropper : MonoBehaviour
{
    [Header("Drop point and prefab")]
    [SerializeField] private GameObject explosiveEggPrefab;
    [SerializeField] private Transform dropPoint;
    [SerializeField] private Vector3 fallbackOffset = new Vector3(0f, -1f, 0f);

    [Header("Ammo & Cooldown")]
    [SerializeField] private int maxEggs = 5;
    [SerializeField] private int startingEggs = 5;
    [SerializeField] private float dropCooldown = 3f;
    [SerializeField] private Key dropKey = Key.E;

    [Header("Automatic Regeneration")]
    [SerializeField] private bool autoRegenerate = true;
    [SerializeField] private float regenInterval = 10f;

    [Header("UI")]
    [SerializeField] private AmmoHUD eggAmmoHUD;

    [Header("Audio")]
    [SerializeField] private AudioClip dropClip;

    private int currentEggs;
    private float nextDropTime = 0f;
    private float regenTimer = 0f;
    private Collider playerCollider;
    private Rigidbody playerRigidbody;

    public int CurrentEggs => currentEggs;
    public int MaxEggs => maxEggs;
    public float RegenProgress => (autoRegenerate && currentEggs < maxEggs && regenInterval > 0f)
        ? Mathf.Clamp01(regenTimer / regenInterval)
        : 0f;

    private void Awake()
    {
        playerCollider = GetComponentInChildren<Collider>();
        playerRigidbody = GetComponent<Rigidbody>();
        currentEggs = Mathf.Clamp(startingEggs, 0, maxEggs);
        RefreshHUD();
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;

        HandleAutoRegeneration();
        HandleDropInput();
    }

    private void HandleAutoRegeneration()
    {
        if (!autoRegenerate || regenInterval <= 0f) return;

        if (currentEggs >= maxEggs)
        {
            regenTimer = 0f;
            return;
        }

        regenTimer += Time.deltaTime;
        if (regenTimer >= regenInterval)
        {
            regenTimer -= regenInterval;
            currentEggs++;
            RefreshHUD();
        }
    }

    private void HandleDropInput()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null || !kb[dropKey].wasPressedThisFrame) return;

        if (currentEggs <= 0)
        {
            return;
        }

        if (Time.time >= nextDropTime)
        {
            DropEgg();
        }
    }

    private void DropEgg()
    {
        if (explosiveEggPrefab == null || currentEggs <= 0) return;

        currentEggs--;
        nextDropTime = Time.time + dropCooldown;
        RefreshHUD();

        Vector3 spawnPos = dropPoint != null ? dropPoint.position : transform.position + fallbackOffset;

        GameObject egg = Instantiate(explosiveEggPrefab, spawnPos, Quaternion.identity);

        Collider eggCollider = egg.GetComponent<Collider>();
        if (playerCollider != null && eggCollider != null)
        {
            Physics.IgnoreCollision(playerCollider, eggCollider);
        }

        Rigidbody eggRb = egg.GetComponent<Rigidbody>();
        if (eggRb != null && playerRigidbody != null)
        {
            Vector3 inheritedVel = playerRigidbody.linearVelocity;
            inheritedVel.y = 0f;
            eggRb.linearVelocity = inheritedVel;
        }

        if (dropClip != null)
        {
            AudioManager.Instance?.PlaySFX(dropClip);
        }
    }

    private void RefreshHUD()
    {
        if (eggAmmoHUD != null)
        {
            eggAmmoHUD.UpdateAmmo(currentEggs);
        }
    }
    
    // Can be used for powerups.
    public void AddEggs(int amount)
    {
        if (amount <= 0) return;
        currentEggs = Mathf.Clamp(currentEggs + amount, 0, maxEggs);
        if (currentEggs >= maxEggs) regenTimer = 0f;
    }

    // Can be used for powerups.
    public void RefillEggs()
    {
        currentEggs = maxEggs;
        regenTimer = 0f;
    }
}