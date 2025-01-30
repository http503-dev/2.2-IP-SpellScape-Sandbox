/*
 * Author: Cyanne Chiang
 * Date: 26/1/2024
 * Description: Script for validating the words in the challenge scene
 */
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;

public class ChallengeValidator : MonoBehaviour
{
    /// <summary>
    /// Correct word that needs to be formed
    /// </summary>
    public string correctWord;

    /// <summary>
    /// Array of sockets where blocks are to be placed
    /// </summary>
    public XRSocketInteractor[] snapPoints;

    /// <summary>
    /// Indicates whether the word has been validated.
    /// </summary>
    private bool isValidated = false;

    /// <summary>
    /// Metrics to be sent to the database (Individual for current word)
    /// </summary>
    public int totalAttempts = 0;
    public int mistakes = 0;
    public float fastestTime = Mathf.Infinity;

    /// <summary>
    /// Time when word formation attempt started
    /// </summary>
    private float startTime;

    /// <summary>
    /// Indicates whether a word is currently being formed
    /// </summary>
    private bool wordInProgress = false;

    /// <summary>
    /// References to particle system/audio source for feedback (correct/incorrect)
    /// </summary>
    public ParticleSystem confettiEffect;
    public AudioSource successSound;
    public AudioSource incorrectSound;

    /// <summary>
    /// Reference to the WordManager
    /// </summary>
    public WordManager wordManager;

    /// <summary>
    /// Adds event listeners for snap points.
    /// </summary>
    private void Start()
    {
        foreach (var socket in snapPoints)
        {
            socket.selectEntered.AddListener(OnBlockPlaced);
            socket.selectExited.AddListener(OnBlockRemoved);
        }
    }

    /// <summary>
    /// Removes event listeners when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        foreach (var socket in snapPoints)
        {
            socket.selectEntered.RemoveListener(OnBlockPlaced);
            socket.selectExited.RemoveListener(OnBlockRemoved);
        }
    }

    /// <summary>
    /// Handles logic when a block is placed into a snap point
    /// </summary>
    /// <param name="args"> The event arguments for the interaction </param>
    private void OnBlockPlaced(SelectEnterEventArgs args)
    {
        if (!wordInProgress)
        {
            wordInProgress = true;
            startTime = Time.time;
        }

        if (!isValidated && snapPoints.All(sp => sp.hasSelection))
        {
            ValidateWord();
            isValidated = true;
        }
    }

    /// <summary>
    /// Handles logic when a block is removed from a snap point
    /// </summary>
    /// <param name="args"> The event arguments for the interaction </param>
    private void OnBlockRemoved(SelectExitEventArgs args)
    {
        isValidated = false;
    }

    /// <summary>
    /// Validates the word formed by the placed blocks
    /// </summary>
    public void ValidateWord()
    {
        string formedWord = string.Concat(snapPoints.Select(sp => sp.GetOldestInteractableSelected()?.transform.name ?? ""));
        totalAttempts++;

        if (formedWord.Equals(correctWord, System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log("Correct Word!");
            TriggerSuccessEffects();

            // Track fastest time
            float timeTaken = Time.time - startTime;
            if (timeTaken < fastestTime)
            {
                fastestTime = timeTaken;
            }

            wordManager.OnWordCompleted(true, timeTaken, mistakes); // Notify WordManager
        }
        else
        {
            Debug.Log("Incorrect Word! Try Again.");
            TriggerIncorrectEffects();
            mistakes++;
        }

        wordInProgress = false;
    }

    /// <summary>
    /// Triggers visual and audio effects on successful word formation
    /// </summary>
    private void TriggerSuccessEffects()
    {
        if (confettiEffect != null) confettiEffect.Play();
        if (successSound != null) successSound.Play();
    }

    /// <summary>
    /// Triggers audio effects on incorrect word formation
    /// </summary>
    private void TriggerIncorrectEffects()
    {
        if (incorrectSound != null) incorrectSound.Play();
    }
}
