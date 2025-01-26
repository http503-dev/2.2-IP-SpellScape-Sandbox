using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;

public class ChallengeValidator : MonoBehaviour
{
    public string correctWord;
    public XRSocketInteractor[] snapPoints;
    private bool isValidated = false;

    // Metrics
    public int totalAttempts = 0;
    public int mistakes = 0;
    public float fastestTime = Mathf.Infinity;
    private float startTime;
    private bool wordInProgress = false;

    public ParticleSystem confettiEffect;
    public AudioSource successSound;
    public AudioSource incorrectSound;

    // Reference to the WordManager
    public WordManager wordManager;

    private void Start()
    {
        foreach (var socket in snapPoints)
        {
            socket.selectEntered.AddListener(OnBlockPlaced);
            socket.selectExited.AddListener(OnBlockRemoved);
        }
    }

    private void OnDestroy()
    {
        foreach (var socket in snapPoints)
        {
            socket.selectEntered.RemoveListener(OnBlockPlaced);
            socket.selectExited.RemoveListener(OnBlockRemoved);
        }
    }

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

    private void OnBlockRemoved(SelectExitEventArgs args)
    {
        isValidated = false;
    }

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

    private void TriggerSuccessEffects()
    {
        if (confettiEffect != null) confettiEffect.Play();
        if (successSound != null) successSound.Play();
    }

    private void TriggerIncorrectEffects()
    {
        if (incorrectSound != null) incorrectSound.Play();
    }
}
