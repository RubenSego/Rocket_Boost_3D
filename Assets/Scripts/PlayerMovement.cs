using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputAction thrust;
    [SerializeField] private InputAction rotation;
    [SerializeField] private float moveForce = 10f;
    [SerializeField] private float rotationForce = 2.5f;
    
    [SerializeField] private ParticleSystem thrustParticleSystem;
    [SerializeField] private ParticleSystem leftThrustParticleSystem;
    [SerializeField] private ParticleSystem rightThrustParticleSystem;

    Rigidbody rb;

    void OnEnable()
    {
        thrust.Enable();
        rotation.Enable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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

    private void RotateLeft()
    {
        transform.Rotate(Vector3.back * rotationForce);
        if (!leftThrustParticleSystem.isPlaying)
        {
            rightThrustParticleSystem.Stop();
            leftThrustParticleSystem.Play();
        }
    }
    private void RotateRight()
    {
        transform.Rotate(Vector3.forward * rotationForce);
        if (!rightThrustParticleSystem.isPlaying)
        {
            leftThrustParticleSystem.Stop();
            rightThrustParticleSystem.Play();
        }
    }
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

    private void StartThrusting()
    {
        //TODO: Active particles, active sound.
        rb.AddRelativeForce(Vector3.up * moveForce);

        if (!thrustParticleSystem.isPlaying)
        {
            thrustParticleSystem.Play();
        }
    }

    private void StopThrusting()
    {
        thrustParticleSystem.Stop();
    }

}