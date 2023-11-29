using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatherJump : MonoBehaviour
{
    
    // Activates a trigger listener
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has collided with the object
        if (other.CompareTag("Player"))
        {
            // Get the ClairMovement script
            ClairMovement clairMovement = other.GetComponent<ClairMovement>();
            
            // Check if the script has been found
            if (clairMovement != null)
            {
                // Increase maxJump
                clairMovement.IncreaseMaxJumps();
                
                // Destroy the object
                Destroy(gameObject);
            }
        }
    }
}
