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

public class WordManager : MonoBehaviour
{
    /// <summary>
    /// Spawn location for the word prefab
    /// </summary>
    public Transform spawnPoint;

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
    private float totalTimeTaken = 0f;
    private int totalMistakes = 0;

    /// <summary>
    /// References to UI elements
    /// </summary>
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    /// <summary>
    /// Initializes the game by setting the difficulty level to Easy and spawns a word
    /// </summary>
    public void Start()
    {
        SetDifficultyLevel(0); // Start at Easy
        SpawnRandomWord();
        UpdateProgressBar(); // Initialize UI
    }

    /// <summary>
    /// Spawns a random word from the available word list at the current difficulty
    /// </summary>
    public void SpawnRandomWord()
    {
        if (currentWord != null)
        {
            Destroy(currentWord); // Remove the previous word
        }

        List<GameObject> currentWordList = GetWordListForDifficulty();

        if (currentWordList.Count == 0)
        {
            Debug.Log($"No words left at difficulty {difficultyLevel}, moving to next level.");
            IncreaseDifficulty();
            return; // Prevent attempting to spawn a new word when the list is empty
        }

        GameObject selectedWord;
        float randomValue = Random.value; // Random number between 0 and 1

        if (difficultyLevel == 1) // Medium difficulty
        {
            if (randomValue < 0.8f && mediumWords.Count > 0) // 80% chance for medium words
            {
                selectedWord = mediumWords[Random.Range(0, mediumWords.Count)];
                mediumWords.Remove(selectedWord);
            }
            else if (easyWords.Count > 0) // 20% chance for easy words
            {
                selectedWord = easyWords[Random.Range(0, easyWords.Count)];
                easyWords.Remove(selectedWord);
            }
            else
            {
                Debug.LogWarning("No words available in lower difficulties.");
                selectedWord = mediumWords[Random.Range(0, mediumWords.Count)];
            }
        }
        else if (difficultyLevel == 2) // Hard difficulty
        {
            if (randomValue < 0.7f && hardWords.Count > 0) // 70% chance for hard words
            {
                selectedWord = hardWords[Random.Range(0, hardWords.Count)];
                hardWords.Remove(selectedWord);
            }
            else if (randomValue < 0.9f && mediumWords.Count > 0) // 20% chance for medium words
            {
                selectedWord = mediumWords[Random.Range(0, mediumWords.Count)];
                mediumWords.Remove(selectedWord);
            }
            else if (easyWords.Count > 0) // 10% chance for easy words
            {
                selectedWord = easyWords[Random.Range(0, easyWords.Count)];
                easyWords.Remove(selectedWord);
            }
            else
            {
                Debug.LogWarning("No words available in lower difficulties.");
                selectedWord = hardWords[Random.Range(0, hardWords.Count)];
            }
        }
        else // Easy difficulty
        {
            selectedWord = easyWords[Random.Range(0, easyWords.Count)];
            easyWords.Remove(selectedWord);
        }

        // Instantiate the selected word prefab
        currentWord = Instantiate(selectedWord, spawnPoint.position, Quaternion.identity);

        var validator = currentWord.GetComponent<ChallengeValidator>();
        if (validator != null)
        {
            validator.wordManager = this;
        }

        Debug.Log($"New word spawned: {currentWord.name}");
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

        // Spawn the next word after a short delay
        Invoke(nameof(SpawnRandomWord), 2f);
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
            LogFinalMetrics();
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
            progressText.text = $"{wordsCompleted}/{wordsToLevelUp} words completed";
        }
    }

    /// <summary>
    /// Logs the final game metrics when all difficulty levels are completed.
    /// </summary>
    public void LogFinalMetrics()
    {
        Debug.Log($"Challenge Complete! Total Words: {totalWordsCompleted}, Total Time: {totalTimeTaken:F2} seconds, Total Mistakes: {totalMistakes}");
    }
}
