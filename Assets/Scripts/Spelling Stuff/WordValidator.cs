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
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class WordValidator : MonoBehaviour
{
    /// <summary>
    /// Reference to correct word that is needed to be formed
    /// </summary>
    public string correctWord;

    /// <summary>
    /// Array of transforms acting as the snap points for the letters
    /// </summary>
    public Transform[] snapPoints;
    private GameObject[] placedBlocks;

    /// <summary>
    /// Flag to check whether word has been validated
    /// </summary>
    private bool isValidated = false;

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
    /// Reference to image tracking manager
    /// </summary>
    public ImageTrackingManager imageTrackingManager;

    /// <summary>
    /// Subscribes to the select and deselect events of all snap points
    /// </summary>
    private void Start()
    {
        // Ensure imageTrackingManager is assigned dynamically if missing
        if (imageTrackingManager == null)
        {
            imageTrackingManager = FindObjectOfType<ImageTrackingManager>();
        }

        if (imageTrackingManager != null)
        {
            imageTrackingManager.RegisterActiveValidator(this);
            Debug.Log($"Registered {correctWord} with ImageTrackingManager.");
        }

        // If difficulty is not unlocked, disable the prefab
        if (imageTrackingManager != null && !imageTrackingManager.CanSpawnPrefab(difficultyLevel))
        {
            Debug.Log($"Word '{correctWord}' is locked! Waiting for difficulty unlock.");
            gameObject.SetActive(false);
            imageTrackingManager.RegisterLockedWord(this); // Register the locked word!
        }

        placedBlocks = new GameObject[snapPoints.Length];
    }

    public bool PlaceBlock(GameObject block, int snapIndex)
    {
        if (snapIndex < 0 || snapIndex >= placedBlocks.Length)
        {
            Debug.LogWarning($"Invalid snap index {snapIndex} for block {block.name}");
            return false;
        }

        if (placedBlocks[snapIndex] != null)
        {
            Debug.LogWarning($"Snap point {snapIndex} already occupied.");
            return false;
        }

        block.transform.position = snapPoints[snapIndex].position;
        block.transform.rotation = snapPoints[snapIndex].rotation;
        block.transform.SetParent(snapPoints[snapIndex]);
        placedBlocks[snapIndex] = block;

        Debug.Log($"Placed {block.name} at snap point {snapIndex}.");

        CheckValidation();
        return true;
    }

    public void RemoveBlock(int snapIndex)
    {
        if (snapIndex < 0 || snapIndex >= placedBlocks.Length || placedBlocks[snapIndex] == null)
        {
            return;
        }

        placedBlocks[snapIndex].transform.SetParent(null);
        placedBlocks[snapIndex] = null;
        isValidated = false;
    }


    private void CheckValidation()
    {
        if (placedBlocks.Any(b => b == null)) return;

        string formedWord = string.Concat(placedBlocks.Select(b => b.name[0])); // Assuming block name is a letter
        if (formedWord.Equals(correctWord, System.StringComparison.OrdinalIgnoreCase))
        {
            TriggerSuccessEffects();
            if (imageTrackingManager != null)
            {
                imageTrackingManager.OnWordCompleted(difficultyLevel);
            }
        }
        else
        {
            TriggerIncorrectEffects();
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
    /// Function to reset validation to false
    /// </summary>
    public void ResetValidation()
    {
        // Reset the validation flag to allow re-validation (e.g., after resetting the game)
        isValidated = false;
    }

    /// <summary>
    /// Function to show locked words
    /// </summary>
    public void ActivateWord()
    {
        gameObject.SetActive(true);
    }
}
