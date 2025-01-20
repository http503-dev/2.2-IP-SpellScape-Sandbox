/*
 * Author: Muhammad Farhan
 * Date: 20/1/2024
 * Description: Script for storing and playing back pronunciations of words
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordAudioPlayer : MonoBehaviour
{
    /// <summary>
    /// References the audio source used for playing audio clips
    /// </summary>
    [SerializeField] private AudioSource audioSource;

    /// <summary>
    /// List of audio clips containing the pronunciation of all words
    /// </summary>
    [SerializeField] private List<AudioClip> wordAudioClips;

    /// <summary>
    /// Dictionary for quick lookup of audio clips by word
    /// </summary>
    private Dictionary<string, AudioClip> audioClipDictionary;

    /// <summary>
    /// Initializes the dictionary for quick audio clip lookups
    /// </summary>
    private void Awake()
    {
        // Create a dictionary for quick lookup
        audioClipDictionary = new Dictionary<string, AudioClip>();

        foreach (var clip in wordAudioClips)
        {
            audioClipDictionary[clip.name.ToLower()] = clip; // Use lowercase for consistency
        }
    }

    /// <summary>
    /// Function to play the pronunciation for a given word
    /// </summary>
    /// <param name="word"> The word to pronounce </param>
    public void PlayAudio(string word)
    {
        // Look for the audio clip in the dictionary
        if (audioClipDictionary.TryGetValue(word.ToLower(), out var clip))
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"Audio clip for '{word}' not found!");
        }
    }
}
