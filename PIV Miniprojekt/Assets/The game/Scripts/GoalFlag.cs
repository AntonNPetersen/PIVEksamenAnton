using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalFlag : MonoBehaviour
{
    public ParticleSystem confettiParticles;

    private void OnTriggerEnter(Collider other)
    {
        // Check if there is collision with the player
        if (other.CompareTag("Player"))
        {
            // Play confetti particles
            if (confettiParticles != null)
            {
                confettiParticles.Play();
            }
        }
    }
}
