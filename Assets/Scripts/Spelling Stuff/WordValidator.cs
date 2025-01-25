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
    /// Array of XR socket interactors representing the snap points for the letters
    /// </summary>
    public XRSocketInteractor[] snapPoints; // Array of sockets to validate

    /// <summary>
    /// Flag to check whether word has been validated
    /// </summary>
    private bool isValidated = false;

    /// <summary>
    /// References to particle system/audio source for feedback (correct/incorrect)
    /// </summary>
    public ParticleSystem confettiEffect;
    public AudioSource successSound;
    public AudioSource incorrectSound;

    /// <summary>
    /// Subscribes to the select and deselect events of all snap points
    /// </summary>
    private void Start()
    {
        // Subscribe to socket events for all snap points
        foreach (var socket in snapPoints)
        {
            socket.selectEntered.AddListener(OnBlockPlaced);
            socket.selectExited.AddListener(OnBlockRemoved);
        }
    }

    /// <summary>
    /// Unsubscribes from the select and deselect events to avoid memory leaks
    /// </summary>
    private void OnDestroy()
    {
        // Unsubscribe from socket events to avoid memory leaks
        foreach (var socket in snapPoints)
        {
            socket.selectEntered.RemoveListener(OnBlockPlaced);
            socket.selectExited.RemoveListener(OnBlockRemoved);
        }
    }

    /// <summary>
    /// Function for when a block is placed in a socket
    /// </summary>
    /// <param name="args"> Event arguments containing details about the interaction </param>
    private void OnBlockPlaced(SelectEnterEventArgs args)
    {
        // Check if all snap points are occupied and validate the word
        if (!isValidated && snapPoints.All(sp => sp.hasSelection))
        {
            ValidateWord();
            isValidated = true;
        }
    }

    /// <summary>
    /// Function for when a block is removed from a socket
    /// </summary>
    /// <param name="args"> Event arguments containing details about the interaction </param>
    private void OnBlockRemoved(SelectExitEventArgs args)
    {
        // Reset the validation state when a block is removed
        isValidated = false;
    }

    /// <summary>
    /// Function to validate the word formed when all letter blocks are placed in all sockets
    /// </summary>
    public void ValidateWord()
    {
        // Collect the names of selected interactables from the snap points
        string formedWord = string.Concat(snapPoints.Select(sp => sp.GetOldestInteractableSelected()?.transform.name ?? ""));

        if (formedWord.Equals(correctWord, System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log("Correct Word!");
            TriggerSuccessEffects();
        }
        else
        {
            Debug.Log("Incorrect Word! Try Again.");
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
}
