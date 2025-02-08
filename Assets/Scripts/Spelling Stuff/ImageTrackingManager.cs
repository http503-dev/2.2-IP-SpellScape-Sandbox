/*
 * Author: Jarene Goh
 * Date: 30/1/2024
 * Description: Script that handles the difficulty scaling of the image tracking scene
 */
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;

public class ImageTrackingManager : MonoBehaviour
{
    /// <summary>
    /// Current difficulty level (0 = Easy, 1 = Medium, 2 = Hard)
    /// </summary>
    private int difficultyLevel = 0;

    /// <summary>
    /// Number of words completed
    /// </summary>
    private int wordsCompleted = 0;

    /// <summary>
    /// Number of words needed to unlock the next difficulty
    /// </summary>
    public int wordsNeededToUnlock = 3;

    /// <summary>
    /// List of words that are locked due to higher difficulty requirements
    /// </summary>
    private List<WordValidator> lockedWords = new List<WordValidator>();

    /// <summary>
    /// UI Elements for Progress Tracking
    /// </summary>
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    /// <summary>
    /// Firebase Database and Auth References
    /// </summary>
    private DatabaseReference database;
    private FirebaseAuth auth;
    private string userId;

    /// <summary>
    /// Initializes the UI Progress Bar at Start and Firebase
    /// </summary>
    private void Start()
    {
        UpdateProgressBar(); // Initialize UI on start

        // Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                database = FirebaseDatabase.DefaultInstance.RootReference;

                if (auth.CurrentUser != null)
                {
                    userId = auth.CurrentUser.UserId;
                    Debug.Log("Firebase Connected - User ID: " + userId);
                }
                else
                {
                    Debug.LogError("No user logged in!");
                }
            }
            else
            {
                Debug.LogError("Could not connect to Firebase: " + task.Result);
            }
        });
    }

    /// <summary>
    /// Function for when a word is completed. Increases the word completion count and checks for difficulty unlocking.
    /// </summary>
    public void OnWordCompleted(int wordDifficulty)
    {
        if (wordDifficulty == difficultyLevel)
        {
            wordsCompleted++;
            Debug.Log($"Words Completed: {wordsCompleted}/3 at Difficulty Level {difficultyLevel}");

            UpdateProgressBar();

            if (wordsCompleted >= wordsNeededToUnlock)
            {
                IncreaseDifficulty();
            }
        }

        else
        {
            Debug.Log($"Word from lower difficulty ({wordDifficulty}) completed, but does NOT count towards leveling up.");
        }
    }

    /// <summary>
    /// Function for increasing difficulty level and unlocking words of said difficulty
    /// </summary>
    private void IncreaseDifficulty()
    {
        if (difficultyLevel < 2)
        {
            difficultyLevel++;
            wordsCompleted = 0; // Reset count for next level

            Debug.Log($"Unlocked difficulty: {difficultyLevel}");

            // Push updated difficulty level to Firebase
            UpdateDifficultyInFirebase();

            // Reactivate locked words that match the new difficulty
            foreach (var word in new List<WordValidator>(lockedWords))
            {
                if (word.difficultyLevel <= difficultyLevel)
                {
                    word.ActivateWord();
                    lockedWords.Remove(word);
                }
            }

            UpdateProgressBar(); // Reset progress bar when difficulty increases
        }
        else
        {
            Debug.Log("All difficulties unlocked!");
        }
    }

    /// <summary>
    /// Checks if a prefab can be spawned based on its required difficulty
    /// </summary>
    /// <param name="requiredDifficulty"> The difficulty level required to spawn the prefab </param>
    /// <returns> True if the required difficulty is less than or equal to the current difficulty level, otherwise false </returns>
    public bool CanSpawnPrefab(int requiredDifficulty)
    {
        return requiredDifficulty <= difficultyLevel;
    }

    /// <summary>
    /// Registers a locked word that will be activated when its difficulty is reached
    /// </summary>
    /// <param name="word"> The word to be locked until the appropriate difficulty is reached </param>
    public void RegisterLockedWord(WordValidator word)
    {
        if (!lockedWords.Contains(word))
        {
            lockedWords.Add(word);
        }
    }

    /// <summary>
    /// Updates the progress bar to show the player's progress towards the next difficulty
    /// </summary>
    private void UpdateProgressBar()
    {
        float progress = (float)wordsCompleted / wordsNeededToUnlock;

        if (progressBar != null)
        {
            progressBar.value = progress; // Update the slider
        }

        if (progressText != null)
        {
            progressText.text = $"{wordsCompleted}/{wordsNeededToUnlock} words completed";
        }
    }

    /// <summary>
    /// Pushes the current difficulty level to Firebase
    /// </summary>
    private void UpdateDifficultyInFirebase()
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("No valid user ID found. Cannot update difficulty.");
            return;
        }

        string difficultyName = GetDifficultyName(difficultyLevel);

        database.Child("users").Child(userId).Child("imageTracking").Child("imageDifficultyLevel").SetValueAsync(difficultyName).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"Image Difficulty Level updated in Firebase: {difficultyName}");
            }
            else
            {
                Debug.LogError($"Failed to update difficulty level: {task.Exception}");
            }
        });
    }

    /// <summary>
    /// Converts difficulty level to a string representation
    /// </summary>
    private string GetDifficultyName(int level)
    {
        return level switch
        {
            0 => "Easy",
            1 => "Medium",
            2 => "Hard",
            _ => "unknown"
        };
    }
}
