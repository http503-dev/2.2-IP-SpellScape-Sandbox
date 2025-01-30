/*
 * Author: Cyanne Chiang
 * Date: 26/1/2024
 * Description: Script that handles the spawning of words and difficulty level for the challenge scene
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    /// List of words that have not yet been used
    /// </summary>
    private List<GameObject> unusedWords;

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
    /// Initializes the game by setting the difficulty level to Easy and spawns a word
    /// </summary>
    public void Start()
    {
        SetDifficultyLevel(0); // Start at Easy
        SpawnRandomWord();
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

        if (unusedWords.Count == 0)
        {
            Debug.Log("No words left at this difficulty, moving to the next level.");
            IncreaseDifficulty();
        }

        if (unusedWords.Count == 0)
        {
            Debug.Log($"No words left at difficulty {difficultyLevel}, moving to next level.");
            IncreaseDifficulty();
            return; // Prevent attempting to spawn a new word when the list is empty
        }

        int randomIndex = Random.Range(0, unusedWords.Count);
        currentWord = Instantiate(unusedWords[randomIndex], spawnPoint.position, Quaternion.identity);
        unusedWords.RemoveAt(randomIndex);

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
            wordsCompleted++;

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
            unusedWords = new List<GameObject>(easyWords);
            Debug.Log("Difficulty set to EASY");
        }
        else if (difficultyLevel == 1)
        {
            unusedWords = new List<GameObject>(mediumWords);
            Debug.Log("Difficulty set to MEDIUM");
        }
        else if (difficultyLevel == 2)
        {
            unusedWords = new List<GameObject>(hardWords);
            Debug.Log("Difficulty set to HARD");
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
