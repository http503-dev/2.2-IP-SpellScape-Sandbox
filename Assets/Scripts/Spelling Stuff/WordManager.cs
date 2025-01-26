using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    public List<GameObject> wordPrefabs; // Prefabs for different words
    public Transform spawnPoint; // Location to spawn the prefab
    private GameObject currentWord;

    private List<GameObject> unusedWords; // Tracks unused words

    // Metrics
    private int totalWordsCompleted = 0;
    private float totalTimeTaken = 0f;
    private int totalMistakes = 0;

    public void Start()
    {
        unusedWords = new List<GameObject>(wordPrefabs);
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
            Debug.Log("All words have been used! Ending the challenge.");
            LogFinalMetrics();
            return;
        }

        // Select a random word from the unused words list
        int randomIndex = Random.Range(0, unusedWords.Count);
        currentWord = Instantiate(unusedWords[randomIndex], spawnPoint.position, Quaternion.identity);

        // Remove the selected word from the unused list
        unusedWords.RemoveAt(randomIndex);

        // Ensure the new word prefab's ChallengeValidator is linked to this WordManager
        var validator = currentWord.GetComponent<ChallengeValidator>();
        if (validator != null)
        {
            validator.wordManager = this; // Set reference
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

            Debug.Log($"Word Completed! Time Taken: {timeTaken:F2} seconds, Mistakes: {mistakes}");
        }

        // Spawn the next word after a short delay
        Invoke(nameof(SpawnRandomWord), 1f);
    }

    public void LogFinalMetrics()
    {
        Debug.Log($"Challenge Complete! Total Words: {totalWordsCompleted}, Total Time: {totalTimeTaken:F2} seconds, Total Mistakes: {totalMistakes}");
    }
}
