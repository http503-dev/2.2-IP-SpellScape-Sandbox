/*
 * Author: Cyanne Chiang
 * Date: 26/1/2024
 * Description: Script that handles the spawning of words and difficulty level for the challenge scene
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Auth;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class WordManager : MonoBehaviour
{
    /// <summary>
    /// AR Components and spawning variables
    /// </summary>
    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();
    private Vector3 fixedSpawnPosition;
    private Quaternion fixedSpawnRotation;
    private bool positionLocked = false; // Track if we already locked a position

    /// <summary>
    /// Current word game object
    /// </summary>
    private GameObject currentWord;

    /// <summary>
    /// Lists of words categorized by difficulty
    /// </summary>
    public List<GameObject> easyWords;
    public List<GameObject> mediumWords;
    public List<GameObject> hardWords;

    /// <summary>
    /// Number of words needed to level up
    /// </summary>
    public int wordsToLevelUp = 2; // To change for builds

    /// <summary>
    /// Number of words successfully completed at the current difficulty level
    /// </summary>
    private int wordsCompleted = 0;

    /// <summary>
    /// Current difficulty level (0 = Easy, 1 = Medium, 2 = Hard)
    /// </summary>
    private int difficultyLevel = 0;

    /// <summary>
    /// Metrics to be stored to database (Overall for that run)
    /// </summary>
    private int totalWordsCompleted = 0;
    private float fastestTime = Mathf.Infinity;
    private int leastMistakes = int.MaxValue;
    private int totalAttempts = 0;
    private float totalTimeTaken = 0f;
    private int totalMistakes = 0;

    /// <summary>
    /// References to UI elements
    /// </summary>
    public Slider progressBar;
    public TextMeshProUGUI progressText;
    public GameObject startPanel;
    public ChallengeTimer challengeTimer;

    /// <summary>
    /// Firebase Database & Authentication References
    /// </summary>
    private DatabaseReference database;
    private FirebaseAuth auth;
    private string userId;

    /// <summary>
    /// Initializes the game by setting the difficulty level to Easy and spawns a word as well as Firebase
    /// </summary>
    public void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();

        // Ensure the Start Panel is active at the beginning
        startPanel.SetActive(true);

        // Pause the game timer until the challenge starts
        if (challengeTimer != null)
        {
            challengeTimer.enabled = false;
        }

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

                    // Increment totalAttempts and update Firebase
                    IncrementTotalAttempts();
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

    public void StartChallenge()
    {
        startPanel.SetActive(false); // Hide the Start Panel
        SetDifficultyLevel(0); // Start at Easy
        StartCoroutine(SpawnRandomWordWithRetry()); // Start spawning with retry
        UpdateProgressBar(); // Initialize UI

        Debug.Log("Challenge started!");
    }
    IEnumerator SpawnRandomWordWithRetry()
    {
        GameObject selectedWord = SelectWordByDifficulty();

        if (selectedWord == null)
        {
            Debug.Log($"No words left at difficulty {difficultyLevel}, moving to next level.");
            IncreaseDifficulty();
            yield break;
        }

        // If position is already locked, skip raycast and spawn at the saved position
        if (positionLocked)
        {
            if (currentWord != null)
            {
                Destroy(currentWord);
            }

            currentWord = Instantiate(selectedWord, fixedSpawnPosition, fixedSpawnRotation);

            var validator = currentWord.GetComponent<ChallengeValidator>();
            if (validator != null)
            {
                validator.wordManager = this;
            }

            Debug.Log($"Spawned '{currentWord.name}' at locked position {fixedSpawnPosition}");
            yield break;
        }

        // If position is not locked, raycast to detect a plane and place the first word
        while (!positionLocked)
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            if (raycastManager.Raycast(screenCenter, raycastHits, TrackableType.PlaneWithinBounds))
            {
                Pose hitPose = raycastHits[0].pose;

                if (currentWord != null)
                {
                    Destroy(currentWord);
                }

                currentWord = Instantiate(selectedWord, hitPose.position, hitPose.rotation);

                var validator = currentWord.GetComponent<ChallengeValidator>();
                if (validator != null)
                {
                    validator.wordManager = this;
                }

                // Lock the position and rotation after the first successful placement
                fixedSpawnPosition = hitPose.position;
                fixedSpawnRotation = hitPose.rotation;
                positionLocked = true;

                Debug.Log($"Spawned '{currentWord.name}' on a detected plane at {hitPose.position} and position LOCKED.");

                // Start the timer only when the **first word** is successfully spawned
                if (challengeTimer != null && !challengeTimer.enabled)
                {
                    challengeTimer.enabled = true;
                    Debug.Log("Timer started after the first word was spawned.");
                }

                yield break;
            }
            else
            {
                Debug.Log("No plane detected. Retrying in 1 second...");
                yield return new WaitForSeconds(1f);
            }
        }
    }
    private GameObject SelectWordByDifficulty()
    {
        List<GameObject> currentList = GetWordListForDifficulty();

        if (currentList.Count == 0)
        {
            return null;
        }

        float randomValue = Random.value;
        GameObject selectedWord = null;

        if (difficultyLevel == 1)
        {
            if (randomValue < 0.8f && mediumWords.Count > 0)
            {
                selectedWord = mediumWords[Random.Range(0, mediumWords.Count)];
                mediumWords.Remove(selectedWord);
            }
            else if (easyWords.Count > 0)
            {
                selectedWord = easyWords[Random.Range(0, easyWords.Count)];
                easyWords.Remove(selectedWord);
            }
        }
        else if (difficultyLevel == 2)
        {
            if (randomValue < 0.7f && hardWords.Count > 0)
            {
                selectedWord = hardWords[Random.Range(0, hardWords.Count)];
                hardWords.Remove(selectedWord);
            }
            else if (randomValue < 0.9f && mediumWords.Count > 0)
            {
                selectedWord = mediumWords[Random.Range(0, mediumWords.Count)];
                mediumWords.Remove(selectedWord);
            }
            else if (easyWords.Count > 0)
            {
                selectedWord = easyWords[Random.Range(0, easyWords.Count)];
                easyWords.Remove(selectedWord);
            }
        }

        // If nothing was selected due to empty lists, default to current difficulty list
        if (selectedWord == null && currentList.Count > 0)
        {
            selectedWord = currentList[Random.Range(0, currentList.Count)];
            currentList.Remove(selectedWord);
        }

        return selectedWord;
    }

    /// <summary>
    /// Function for when a word is completed, updates statistics and checks for difficulty progression.
    /// </summary>
    /// <param name="success"> Whether the word was completed successfully </param>
    /// <param name="timeTaken"> Time taken to complete the word </param>
    /// <param name="mistakes"> Number of mistakes made </param>
    public void OnWordCompleted(bool success, float timeTaken, int mistakes)
    {
        if (success)
        {
            totalWordsCompleted++;
            totalTimeTaken += timeTaken;
            totalMistakes += mistakes;

            // Update session-level stats
            if (timeTaken < fastestTime)
            {
                fastestTime = timeTaken;
            }

            if (mistakes < leastMistakes)
            {
                leastMistakes = mistakes;
            }

            // Access wordDifficulty from the current word's ChallengeValidator
            var validator = currentWord.GetComponent<ChallengeValidator>();
            if (validator != null)
            {
                // Check if the word's difficulty matches the current difficulty level
                if (validator.wordDifficulty == difficultyLevel)
                {
                    wordsCompleted++; // Only count words that match the current difficulty
                    UpdateProgressBar(); // Update progress bar on word completion
                }
                else
                {
                    Debug.Log($"Word completed was not of the current difficulty ({difficultyLevel}), so it doesn't count towards progress.");
                }
            }

            Debug.Log($"Word Completed! Time Taken: {timeTaken:F2} seconds, Mistakes: {mistakes}");


            if (wordsCompleted >= wordsToLevelUp)
            {
                IncreaseDifficulty();
            }
        }

        StartCoroutine(PlayEffectsAndSpawnNextWord());
    }

    private IEnumerator PlayEffectsAndSpawnNextWord()
    {
        // Adjust the delay time based on your effect duration (e.g., 2 seconds)
        yield return new WaitForSeconds(3f);

        // After effects finish, destroy the word and spawn the next one
        if (currentWord != null)
        {
            Destroy(currentWord);
        }

        StartCoroutine(SpawnRandomWordWithRetry());
    }

    /// <summary>
    /// Increases the difficulty level if possible, otherwise logs completion metrics.
    /// </summary>
    private void IncreaseDifficulty()
    {
        if (difficultyLevel == 0)
        {
            SetDifficultyLevel(1);
        }
        else if (difficultyLevel == 1)
        {
            SetDifficultyLevel(2);
        }
        else
        {
            Debug.Log("All difficulty levels completed!");
        }

        UpdateProgressBar(); // Reset progress bar on difficulty increase
    }

    /// <summary>
    /// Sets the difficulty level and initializes the corresponding word list.
    /// </summary>
    /// <param name="level"> The difficulty level to set </param>
    private void SetDifficultyLevel(int level)
    {
        difficultyLevel = level;
        wordsCompleted = 0;

        if (difficultyLevel == 0)
        {
            Debug.Log("Difficulty set to EASY" + level);
        }
        else if (difficultyLevel == 1)
        {
            Debug.Log("Difficulty set to MEDIUM" + level);
        }
        else if (difficultyLevel == 2)
        {
            Debug.Log("Difficulty set to HARD" + level);
        }

        UpdateProgressBar(); // Reset progress bar when difficulty changes
    }

    /// <summary>
    /// Gets the word list corresponding to the current difficulty
    /// </summary>
    private List<GameObject> GetWordListForDifficulty()
    {
        if (difficultyLevel == 0)
        {
            return easyWords;
        }
        else if (difficultyLevel == 1)
        {
            return mediumWords;
        }
        else if (difficultyLevel == 2)
        {
            return hardWords;
        }
        else
        {
            return new List<GameObject>(); // Empty list for invalid levels
        }
    }

    /// <summary>
    /// Function to update progress bar (words needed to increase difficulty level)
    /// </summary>
    public void UpdateProgressBar()
    {
        float progress = (float)wordsCompleted / wordsToLevelUp;
        if (progressBar != null)
        {
            progressBar.value = progress;
        }

        if (progressText != null)
        {
            progressText.text = $"{wordsCompleted}/{wordsToLevelUp} words completed\nto unlock next difficulty";
        }
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
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Function to increment total attempts
    /// </summary>
    private void IncrementTotalAttempts()
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("No valid user ID found");
            return;
        }

        // Get the current value of totalAttempts from Firebase
        database.Child("users").Child(userId).Child("challengeArea").Child("totalAttempts").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Failed to retrieve totalAttempts: " + task.Exception);
                return;
            }

            int currentAttempts = 0;
            if (task.Result.Exists && int.TryParse(task.Result.Value.ToString(), out currentAttempts))
            {
                Debug.Log($"Current totalAttempts: {currentAttempts}");
            }

            // Increment the totalAttempts
            int updatedAttempts = currentAttempts + 1;

            // Update Firebase with the incremented value
            database.Child("users").Child(userId).Child("challengeArea").Child("totalAttempts").SetValueAsync(updatedAttempts).ContinueWithOnMainThread(updateTask =>
            {
                if (updateTask.IsCompleted)
                {
                    Debug.Log($"totalAttempts successfully incremented to {updatedAttempts} in Firebase!");
                }
                else
                {
                    Debug.LogError("Failed to increment totalAttempts: " + updateTask.Exception);
                }
            });
        });
    }

    /// <summary>
    /// Logs the final game metrics when all difficulty levels are completed.
    /// </summary>
    public void LogFinalMetrics()
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("No valid user ID found");
            return;
        }

        // Calculate required metrics
        float fastestTimePerWord = fastestTime; 
        int leastMistakesOverall = leastMistakes; 
        int totalWordsPerAttempt = totalWordsCompleted;
        string challengeDifficultyLevel = GetDifficultyName(difficultyLevel);

        // Prepare challenge stats
        Dictionary<string, object> challengeStats = new Dictionary<string, object>
    {
        { "fastestTimePerWord", fastestTimePerWord },
        { "leastMistakes", leastMistakesOverall },
        { "totalWordsPerAttempt", totalWordsPerAttempt },
        { "challengeDifficultyLevel", challengeDifficultyLevel }
    };

        // Log metrics to Firebase under Challenge Area
        database.Child("users").Child(userId).Child("challengeArea").UpdateChildrenAsync(challengeStats).ContinueWithOnMainThread(updateTask =>
        {
            if (updateTask.IsCompleted)
            {
                Debug.Log("Challenge stats successfully uploaded to Firebase!");
            }
            else
            {
                Debug.LogError("Failed to upload challenge stats: " + updateTask.Exception);
            }
        });

        Debug.Log($"Challenge Complete! Fastest Time Per Word: {fastestTimePerWord:F2}s, Least Mistakes: {leastMistakesOverall}, Total Attempts: {totalAttempts}, Total Words Per Attempt: {totalWordsPerAttempt}, Difficulty Level: {challengeDifficultyLevel}");
    }
}
