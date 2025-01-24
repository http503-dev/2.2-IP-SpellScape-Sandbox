/*
 * Author: Jarene Goh
 * Date: 22/1/2024
 * Description: Script that loads and manages a large English dictionary for word validation using HashSet for optimized lookups.
 */
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WordDictionaryLoader : MonoBehaviour
{
    /// <summary>
    /// Singleton instance to provide global access to the dictionary loader
    /// </summary>
    public static WordDictionaryLoader Instance { get; private set; }

    /// <summary>
    /// A HashSet containing all the words loaded from the dictionary file for optimized word lookups
    /// </summary>
    private HashSet<string> wordSet; // Optimized structure for fast lookups

    /// <summary>
    /// Ensures the dictionary loader persists across scenes and initializes the dictionary on startup
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist dictionary across scenes
            LoadDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Loads the dictionary from the .txt file and adds it to HashSet
    /// </summary>
    private void LoadDictionary()
    {
        // Path to the dictionary file in StreamingAssets
        string filePath = Path.Combine(Application.streamingAssetsPath, "words_alpha.txt");

        if (File.Exists(filePath))
        {
            try
            {
                // Read all lines and populate the HashSet
                string[] lines = File.ReadAllLines(filePath);
                wordSet = new HashSet<string>(lines);
                Debug.Log($"Loaded {wordSet.Count} words into the dictionary.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error loading dictionary: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"Dictionary file not found at: {filePath}");
        }
    }

    /// <summary>
    /// Checks if a word exists in the dictionary
    /// </summary>
    /// <param name="word">The word to validate</param>
    /// <returns>True if the word exists, otherwise false</returns>
    public bool IsValidWord(string word)
    {
        return wordSet != null && wordSet.Contains(word.ToLower());
    }
}

