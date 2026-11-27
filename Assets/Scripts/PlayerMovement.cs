using Microlight.MicroBar;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input Actions")] 
    [SerializeField] private InputAction thrust;
    [SerializeField] private InputAction rotation;
    [SerializeField] private InputAction move;
    [SerializeField] private InputAction shoot;

    [Header("Movement Settings")] 
    [SerializeField] private float moveForce = 10f;
    [SerializeField] private float rotationForce = 2.5f;
    [SerializeField] private float walkSpeed = 4f;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 25f;
    [SerializeField] private float groundRegenRate = 35f;
    [SerializeField] private float airRegenRate = 12f;
    [SerializeField] private float minStaminaToThrust = 5f;
    [SerializeField] private MicroBar staminaBar;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private float groundTurnSpeed = 12f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Particle Systems")] 
    [SerializeField] private ParticleSystem thrustParticleSystem;
    [SerializeField] private ParticleSystem leftThrustParticleSystem;
    [SerializeField] private ParticleSystem rightThrustParticleSystem;

    Rigidbody rb;
    AudioSource audioSource;
    PlayerShooting playerShooting;
    Animator animator;
    private bool isGrounded;
    private float currentStamina;
    private bool canThrust = true;

    void OnEnable()
    {
        thrust.Enable();
        rotation.Enable();
        shoot.Enable();
        move.Enable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        playerShooting = GetComponent<PlayerShooting>();
        animator = GetComponentInChildren<Animator>();
        currentStamina = maxStamina;
        if (staminaBar != null)
        {
            staminaBar.Initialize(maxStamina);
        }
    }

    void OnDisable()
    {
        thrust.Disable();
        rotation.Disable();
        shoot.Disable();
        move.Disable();
    }

    void FixedUpdate()
    {
        CheckGround();

        bool wantsToThrust = thrust.IsPressed();
        bool activeThrust = wantsToThrust && canThrust && currentStamina > 0.01f;

        UpdateStamina(activeThrust);

        if (activeThrust)
        {
            ProcessThrust(activeThrust);
            ProcessRotation();
            UpdateAirFacing();
        }
        else if (isGrounded)
        {
            StopThrusting();
            StopRotation();
            UpdateAirFacing();
            ProcessGroundMovement();
        }
        else
        {
            ProcessThrust(activeThrust);
            ProcessRotation();
            UpdateAirFacing();
            UpdateGroundAnimator(false);
        }

        ProcessShooting();
    }

    private void CheckGround()
    {
        Vector3 origin = groundCheck != null
            ? groundCheck.position
            : transform.position + Vector3.down * 0.5f;

        isGrounded = Physics.CheckSphere(origin, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }

    private void ProcessGroundMovement()
    {
        rb.freezeRotation = true;

        float moveInput = move.ReadValue<float>();
        bool isMovingOnGround = Mathf.Abs(moveInput) > 0.01f;

        rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.identity, groundTurnSpeed * Time.fixedDeltaTime));
        UpdateVisualFacing(moveInput);

        Vector3 vel = rb.linearVelocity;
        vel.x = moveInput * walkSpeed;
        rb.linearVelocity = vel;

        UpdateGroundAnimator(isMovingOnGround);
    }

    private void UpdateGroundAnimator(bool isRunning)
    {
        if (animator == null) return;
        animator.SetBool("isRunning", isRunning);
    }

    private void UpdateAirFacing()
    {
        float airTurnInput = rotation.ReadValue<float>();
        if (Mathf.Abs(airTurnInput) <= 0.01f)
        {
            airTurnInput = move.ReadValue<float>();
        }
        UpdateVisualFacing(airTurnInput);
    }

    /// <summary>
    /// Processes the shooting input and triggers the shooting action if the shoot input is triggered.
    /// </summary>
    private void ProcessShooting()
    {
        if (shoot.IsPressed()) playerShooting.Shoot();
    }

    private void ProcessRotation()
    {
        rb.freezeRotation = true; // Freezing rotation so we can manually rotate.

        float rotationValue = rotation.ReadValue<float>();
        if (rotationValue < 0)
        {
            RotateRight();
        }
        else if (rotationValue > 0)
        {
            RotateLeft();
        }
        else StopRotation();
    }

    /// <summary>
    /// Rotates the player object to the left and manages the particle systems for left and right thrust effects.
    /// </summary>
    private void RotateLeft()
    {
        transform.Rotate(Vector3.back * rotationForce);
        if (!leftThrustParticleSystem.isPlaying)
        {
            rightThrustParticleSystem.Stop();
            leftThrustParticleSystem.Play();
        }
    }
    /// <summary>
    /// Rotates the player object to the right and manages the particle systems for left and right thrust effects.
    /// </summary>
    private void RotateRight()
    {
        transform.Rotate(Vector3.forward * rotationForce);
        if (!rightThrustParticleSystem.isPlaying)
        {
            leftThrustParticleSystem.Stop();
            rightThrustParticleSystem.Play();
        }
    }
    /// <summary>
    /// Stops the rotation action by stopping the left and right thrust particle systems.
    /// </summary>
    private void StopRotation()
    {
        rightThrustParticleSystem.Stop();
        leftThrustParticleSystem.Stop();            
    }

    private void ProcessThrust(bool canApplyThrust)
    {
        if (canApplyThrust)
        {
            StartThrusting();
            SetFlying(true);
        }
        else
        {
            StopThrusting();
            SetFlying(false);
        }
    }
    /// <summary>
    /// Starts the thrusting action by playing the particle system and audio source.
    /// </summary>
    private void StartThrusting()
    {
        if (!canThrust || currentStamina <= 0.01f)
        {
            StopThrusting();
            return;
        }
        rb.AddRelativeForce(Vector3.up * moveForce);

        if (!thrustParticleSystem.isPlaying)
        {
            thrustParticleSystem.Play();
        }
    }

    /// <summary>
    /// Stops the thrusting action by stopping the particle system and audio source.
    /// </summary>
    private void StopThrusting()
    {
        if (thrustParticleSystem != null) thrustParticleSystem.Stop();
        SetFlying(false);
    }

    private void SetFlying(bool flying)
    {
        if (animator != null)
        {
            animator.SetBool("isFlying", flying);
        }
    }

    public void ResetMovementState()
    {
        StopThrusting();
        StopRotation();
        SetFlying(false);
        UpdateGroundAnimator(false);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 origin = groundCheck != null
            ? groundCheck.position
            : transform.position + Vector3.down * 0.5f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, groundCheckRadius);
    }

    private void UpdateVisualFacing(float horizontalInput)
    {
        if (animator == null) return;

        float targetY = animator.transform.localEulerAngles.y;
        if (horizontalInput > 0.01f)
            targetY = 90f;
        else if (horizontalInput < -0.01f)
            targetY = -90f;

        Quaternion targetVisualRot = Quaternion.Euler(0f, targetY, 0f);
        animator.transform.localRotation = Quaternion.Slerp(
            animator.transform.localRotation,
            targetVisualRot,
            groundTurnSpeed * Time.fixedDeltaTime
        );
    }

    private void UpdateStamina(bool isThrusting)
    {
        float previousStamina = currentStamina;

        if (isThrusting)
        {
            currentStamina = Mathf.Max(0f, currentStamina - staminaDrainRate * Time.fixedDeltaTime);
            if (currentStamina <= 0.01f)
            {
                canThrust = false;
            }
        }
        else
        {
            float regenRate = isGrounded ? groundRegenRate : airRegenRate;
            currentStamina = Mathf.Min(maxStamina, currentStamina + regenRate * Time.fixedDeltaTime);

            if (!canThrust && currentStamina >= minStaminaToThrust)
            {
                canThrust = true;
            }
        }

        if (staminaBar != null && !Mathf.Approximately(currentStamina, previousStamina))
        {
            UpdateAnim animType = currentStamina < previousStamina ? UpdateAnim.Damage : UpdateAnim.Heal;
            staminaBar.UpdateBar(currentStamina, animType);
        }
    }
}