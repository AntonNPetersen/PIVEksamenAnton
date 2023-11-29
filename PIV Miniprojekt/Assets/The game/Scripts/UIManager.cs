using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public ClairMovement clairMovement; // Drag and drop your ClairMovement script here in the Inspector

    public Image[] maxJumpsImages; // Array of UI Image components for maxJumps
    public Image[] remainingJumpsImages; // Array of UI Image components for remainingJumps
    
    void Start()
    {
        // Subscribe to ClairMovement events
        if (clairMovement != null)
        {
            clairMovement.OnJumpsUpdate += UpdateUI;
        }
    }

    void UpdateUI(int maxJumps, int remainingJumps)
    {
        // Activate images based on maxJumps
        for (int i = 0; i < maxJumpsImages.Length; i++)
        {
            remainingJumpsImages[i].gameObject.SetActive(i < maxJumps);
        }

        // Change images based on remainingJumps
        for (int i = 0; i < remainingJumpsImages.Length; i++)
        {
            maxJumpsImages[i].gameObject.SetActive(i < remainingJumps);
        }
    }
}