/*
 * Author: Muhammad Farhan
 * Date: 20/1/2024
 * Description: Script for validating words for the image tracking scene
 */
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class WordValidator : MonoBehaviour
{
    /// <summary>
    /// Reference to correct word that is needed to be formed
    /// </summary>
    public string correctWord;

    /// <summary>
    /// References to particle system/audio source for feedback (correct/incorrect)
    /// </summary>
    public ParticleSystem confettiEffect;
    public ParticleSystem failEffect;
    public AudioSource successSound;
    public AudioSource incorrectSound;

    /// <summary>
    /// The difficulty level of the word (0 = Easy, 1 = Medium, 2 = Hard)
    /// </summary>
    public int difficultyLevel;

    /// <summary>
    /// Reference to Image Tracking Manager
    /// </summary>
    public ImageTrackingManager imageTrackingManager;


    /// <summary>
    /// Assigns the Image tracking Manager dynamically and locks words of higher difficulties
    /// </summary>
    private void Start()
    {
        // Ensure imageTrackingManager is assigned dynamically if missing
        if (imageTrackingManager == null)
        {
            imageTrackingManager = FindObjectOfType<ImageTrackingManager>();
        }

        // If difficulty is not unlocked, disable the prefab
        if (imageTrackingManager != null && !imageTrackingManager.CanSpawnPrefab(difficultyLevel))
        {
            Debug.Log($"Word '{correctWord}' is locked! Waiting for difficulty unlock.");
            gameObject.SetActive(false);
            imageTrackingManager.RegisterLockedWord(this); // Register the locked word!
        }
    }

    /// <summary>
    /// Triggers success feedback, including confetti and sound.
    /// </summary>
    private void TriggerSuccessEffects()
    {
        // Play confetti effect if assigned
        if (confettiEffect != null)
        {
            confettiEffect.Play();
        }

        // Play success sound if assigned
        if (successSound != null)
        {
            successSound.Play();
        }
    }

    /// <summary>
    /// Triggers feedback for incorrect word, including a sound.
    /// </summary>
    private void TriggerIncorrectEffects()
    {
        // Play fail effect if assigned
        if (failEffect != null)
        {
            failEffect.Play();
        }

        // Play incorrect sound if assigned
        if (incorrectSound != null)
        {
            incorrectSound.Play();
        }
    }

    /// <summary>
    /// Function to show locked words
    /// </summary>
    public void ActivateWord()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Function to validate the word being formed
    /// </summary>
    /// <param name="formedWord"> The word being formed </param>
    public void ValidateCustomWord(string formedWord)
    {
        if (formedWord.Equals(correctWord, System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log("Correct Word!");
            TriggerSuccessEffects();

            if (imageTrackingManager != null)
            {
                imageTrackingManager.OnWordCompleted(difficultyLevel);
            }
        }
        else
        {
            Debug.Log("Incorrect Word! Try Again.");
            TriggerIncorrectEffects();
        }
    }

    /// <summary>
    /// Function to reset the word in case of a failed attempt
    /// </summary>
    public void ResetWord()
    {
        // Reset all blocks
        ImageBlock[] blocks = GetComponentsInChildren<ImageBlock>();
        foreach (var block in blocks)
        {
            block.ResetBlock();
        }

        // Clear all slots
        WordSlot[] slots = GetComponentsInChildren<WordSlot>();
        foreach (var slot in slots)
        {
            slot.ClearSlot();
        }
    }
}
