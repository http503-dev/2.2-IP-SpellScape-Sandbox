using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    public Transform spawnPoint; // Location to spawn the prefab
    private GameObject currentWord;

    private List<GameObject> unusedWords; // Tracks unused words

    public List<GameObject> easyWords;
    public List<GameObject> mediumWords;
    public List<GameObject> hardWords;

    private int wordsCompleted = 0;
    private int difficultyLevel = 0; // 0 = Easy, 1 = Medium, 2 = Hard
    private int wordsToLevelUp = 2; // Number of words needed to progress

    // Metrics
    private int totalWordsCompleted = 0;
    private float totalTimeTaken = 0f;
    private int totalMistakes = 0;

    public void Start()
    {
        SetDifficultyLevel(0); // Start at Easy
        SpawnRandomWord();
    }

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
        Invoke(nameof(SpawnRandomWord), 1f);
    }

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

    public void LogFinalMetrics()
    {
        Debug.Log($"Challenge Complete! Total Words: {totalWordsCompleted}, Total Time: {totalTimeTaken:F2} seconds, Total Mistakes: {totalMistakes}");
    }
}
