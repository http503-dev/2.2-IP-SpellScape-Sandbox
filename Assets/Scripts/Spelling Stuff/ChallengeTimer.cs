using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChallengeTimer : MonoBehaviour
{
    [SerializeField]
    public float totalTime; // Total time for the quiz in seconds
    private float currentTime;
    public TextMeshProUGUI timerText; // UI Text element to display the timer
    public GameObject gameOverPanel; // Panel to display when time is up
    private bool isRunning = true;

    private void Start()
    {
        currentTime = totalTime;
        UpdateTimerDisplay();
    }

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

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{minutes:D2}:{seconds:D2}";
    }

    private void EndChallenge()
    {
        isRunning = false;
        timerText.text = "00:00";
        gameOverPanel.SetActive(true); // Show Game Over UI
        Debug.Log("Time's up! Challenge over.");
    }
}
