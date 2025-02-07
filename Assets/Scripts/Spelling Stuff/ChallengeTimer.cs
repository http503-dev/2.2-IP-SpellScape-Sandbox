/*
 * Author: Zhi Qian
 * Date: 26/1/2024
 * Description: Timer for challenge scene
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChallengeTimer : MonoBehaviour
{
    /// <summary>
    /// Total time for the quiz in seconds
    /// </summary>
    [SerializeField]
    public float totalTime;

    /// <summary>
    /// Current time remaining
    /// </summary>
    private float currentTime;

    /// <summary>
    /// UI Text element to display the timer
    /// </summary>
    public TextMeshProUGUI timerText;

    /// <summary>
    /// Panel to display when time is up
    /// </summary>
    public GameObject gameOverPanel;

    /// <summary>
    /// Bool indicating whether the timer is running
    /// </summary>
    private bool isRunning = true;

    /// <summary>
    /// Initializes the timer with the total time and updates the UI
    /// </summary>
    private void Start()
    {
        currentTime = totalTime;
        UpdateTimerDisplay();
        gameOverPanel.SetActive(false);
    }

    /// <summary>
    /// Updates the timer every frame and ends the challenge when time runs out
    /// </summary>
    private void Update()
    {
        if (isRunning)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
                EndChallenge();
            }

            UpdateTimerDisplay();
        }
    }

    /// <summary>
    /// Updates the timer display in minutes and seconds format
    /// </summary>
    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{minutes:D2}:{seconds:D2}";
    }

    /// <summary>
    /// Ends the challenge when the time is up
    /// </summary>
    private void EndChallenge()
    {
        isRunning = false;
        timerText.text = "00:00";
        gameOverPanel.SetActive(true); // Show Game Over UI
        Debug.Log("Time's up! Challenge over.");
    }
}
