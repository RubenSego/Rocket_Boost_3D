using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input Actions")] 
    [SerializeField] private InputAction thrust;
    [SerializeField] private InputAction rotation;

    [Header("Movement Settings")] 
    [SerializeField] private float moveForce = 10f;
    [SerializeField] private float rotationForce = 2.5f;

    [Header("Particle Systems")] 
    [SerializeField] private ParticleSystem thrustParticleSystem;
    [SerializeField] private ParticleSystem leftThrustParticleSystem;
    [SerializeField] private ParticleSystem rightThrustParticleSystem;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip thrustAudioClip;

    Rigidbody rb;
    AudioSource audioSource;

    void OnEnable()
    {
        thrust.Enable();
        rotation.Enable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        ProcessThrust();
        ProcessRotation();
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

    private void ProcessThrust()
    {
        if (thrust.IsPressed())
        {
            StartThrusting();
        }
        else
        {
            StopThrusting();
        }
    }
    /// <summary>
    /// Starts the thrusting action by playing the particle system and audio source.
    /// </summary>
    private void StartThrusting()
    {
        rb.AddRelativeForce(Vector3.up * moveForce);

        if (!thrustParticleSystem.isPlaying)
        {
            thrustParticleSystem.Play();
            audioSource.PlayOneShot(thrustAudioClip);
        }
    }

    /// <summary>
    /// Stops the thrusting action by stopping the particle system and audio source.
    /// </summary>
    private void StopThrusting()
    {
        thrustParticleSystem.Stop();
        audioSource.Stop();
    }

}