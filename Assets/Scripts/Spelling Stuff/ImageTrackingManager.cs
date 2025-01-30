/*
 * Author: Jarene Goh
 * Date: 30/1/2024
 * Description: Script that handles the difficulty scaling of the image tracking scene
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private int wordsNeededToUnlock = 3;

    /// <summary>
    /// List of words that are locked due to higher difficulty requirements
    /// </summary>
    private List<WordValidator> lockedWords = new List<WordValidator>();

    /// <summary>
    /// Function for when a word is completed. Increases the word completion count and checks for difficulty unlocking.
    /// </summary>
    public void OnWordCompleted()
    {
        wordsCompleted++;
        Debug.Log($"Words Completed: {wordsCompleted}/3 at Difficulty Level {difficultyLevel}");

        if (wordsCompleted >= wordsNeededToUnlock)
        {
            IncreaseDifficulty();
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

            // Reactivate locked words that match the new difficulty
            foreach (var word in new List<WordValidator>(lockedWords))
            {
                if (word.difficultyLevel <= difficultyLevel)
                {
                    word.ActivateWord();
                    lockedWords.Remove(word);
                }
            }
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
}
