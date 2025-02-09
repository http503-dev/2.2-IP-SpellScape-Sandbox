/*
 * Author: Muhammad Farhan
 * Date: 22/1/2024
 * Description: Script that handles registration of sockets, tracking attached letter blocks, and word validation (dictionary from WordDictionaryLoader)
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;

public class WordDiscovery : MonoBehaviour
{
    /// <summary>
    /// A list to track all registered XRSocketInteractors in the scene
    /// </summary>
    private List<XRSocketInteractor> registeredSockets = new List<XRSocketInteractor>();

    /// <summary>
    /// A HashSet to track unique words discovered
    /// </summary>
    private HashSet<string> uniqueWords = new HashSet<string>();

    /// <summary>
    /// Firebase Database and Authentication References
    /// </summary>
    private DatabaseReference database;
    private FirebaseAuth auth;
    private string userId;

    /// <summary>
    /// References to UI elements
    /// </summary>
    public TextMeshProUGUI formedText;

    /// <summary>
    /// Automatically registers any existing sockets in the scene and initializes Firebase
    /// </summary>
    private void OnEnable()
    {
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

                    // Retrieve unique words from Firebase
                    LoadUniqueWordsFromFirebase();
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
        // Register any sockets already in the scene
        foreach (var socket in FindObjectsOfType<XRSocketInteractor>())
        {
            if (!registeredSockets.Contains(socket))
            {
                RegisterSocket(socket);
            }
        }
    }

    /// <summary>
    /// Unregisters all sockets to prevent memory leaks
    /// </summary>
    private void OnDisable()
    {
        // Unregister all sockets to avoid memory leaks
        foreach (var socket in registeredSockets)
        {
            UnregisterSocket(socket);
        }
        registeredSockets.Clear();
    }

    /// <summary>
    /// Registers a new socket and listens for interactions with attached blocks
    /// </summary>
    /// <param name="socket">The socket to register</param>
    public void RegisterSocket(XRSocketInteractor socket)
    {
        if (!registeredSockets.Contains(socket))
        {
            registeredSockets.Add(socket);
            socket.selectEntered.AddListener(OnBlockAttached);
            socket.selectExited.AddListener(OnBlockDetached);
            Debug.Log($"Socket registered: {socket.name}");
        }
    }

    /// <summary>
    /// Unregisters a socket, removing event listeners to prevent memory leaks
    /// </summary>
    /// <param name="socket">The socket to unregister</param>
    public void UnregisterSocket(XRSocketInteractor socket)
    {
        if (registeredSockets.Contains(socket))
        {
            socket.selectEntered.RemoveListener(OnBlockAttached);
            socket.selectExited.RemoveListener(OnBlockDetached);
            registeredSockets.Remove(socket);
            Debug.Log($"Socket unregistered: {socket.name}");
        }
    }

    /// <summary>
    /// Called when a block is attached to any socket
    /// </summary>
    /// <param name="args">The event arguments containing details of the interaction</param>
    private void OnBlockAttached(SelectEnterEventArgs args)
    {
        var attachedBlock = args.interactableObject.transform.GetComponent<LetterBlock>();
        var parentSocket = args.interactorObject.transform; // Socket being interacted with
        var parentBlock = parentSocket.GetComponentInParent<LetterBlock>(); // Get LetterBlock from parent hierarchy

        if (attachedBlock != null)
        {
            Debug.Log($"Block attached: {attachedBlock.letter}");

            // Link the attached block to its left parent
            if (parentBlock != null)
            {
                attachedBlock.connectedToLeft = parentBlock;
                Debug.Log($"Block {attachedBlock.letter} connected to {parentBlock.letter}");
            }
            else
            {
                Debug.Log($"Block {attachedBlock.letter} is not connected to any other block.");
            }

            // Always start from the root block to validate the word
            LetterBlock rootBlock = attachedBlock.GetRootBlock();
            string formedWord = rootBlock.GetFormedWord();
            ValidateWord(formedWord);
        }
        else
        {
            Debug.LogError("Attached object is not a LetterBlock!");
        }
    }

    /// <summary>
    /// Called when a block is detached from any socket.
    /// </summary>
    /// <param name="args">The event arguments containing details of the interaction.</param>
    private void OnBlockDetached(SelectExitEventArgs args)
    {
        var detachedBlock = args.interactableObject.transform.GetComponent<LetterBlock>();

        if (detachedBlock != null)
        {
            // Clear the connectedToLeft field of the detached block
            detachedBlock.connectedToLeft = null;
            Debug.Log($"Block detached: {detachedBlock.letter}");
        }
        else
        {
            Debug.LogError("Detached object is not a LetterBlock!");
        }
    }

    /// <summary>
    /// Validates the word against the dictionary
    /// </summary>
    private void ValidateWord(string word)
    {
        if (WordDictionaryLoader.Instance.IsValidWord(word))
        {
            Debug.Log($"Valid word formed: {word}");
            formedText.text = $"Valid word formed: \n {word}\nbut is not unique";

            // Add word to uniqueWords and update Firebase
            if (uniqueWords.Add(word))
            {
                Debug.Log($"Unique word discovered: {word}");
                formedText.text = $"Unique word discovered: \n {word}";
                UpdateUniqueWordsInFirebase();
            }
        }
        else
        {
            Debug.Log($"Invalid word: {word}");
            formedText.text = $"Invalid word: \n {word}";
        }
    }

    /// <summary>
    /// Retrieves the list of unique words from Firebase and populates the HashSet
    /// </summary>
    private void LoadUniqueWordsFromFirebase()
    {
        database.Child("users").Child(userId).Child("sandboxArea").Child("uniqueWordsList").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Failed to load unique words from Firebase: {task.Exception}");
                return;
            }

            if (task.Result.Exists)
            {
                foreach (var word in task.Result.Children)
                {
                    uniqueWords.Add(word.Value.ToString());
                }

                Debug.Log($"Loaded {uniqueWords.Count} unique words from Firebase.");
            }
            else
            {
                Debug.Log("No unique words found in Firebase.");
            }
        });
    }

    /// <summary>
    /// Updates the number of unique words discovered in Firebase and stores the full list
    /// </summary>
    private void UpdateUniqueWordsInFirebase()
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("No valid user ID found. Cannot update unique words.");
            return;
        }

        int uniqueWordCount = uniqueWords.Count;

        // Prepare list of unique words to store in Firebase
        Dictionary<string, object> wordsDictionary = new Dictionary<string, object>();
        int index = 0;
        foreach (string word in uniqueWords)
        {
            wordsDictionary[index.ToString()] = word;
            index++;
        }

        // Update uniqueWords count and word list in Firebase
        var sandboxUpdates = new Dictionary<string, object>
    {
        { "uniqueWords", uniqueWordCount },
        { "uniqueWordsList", wordsDictionary }
    };

        database.Child("users").Child(userId).Child("sandboxArea").UpdateChildrenAsync(sandboxUpdates).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"Unique word count and list updated in Firebase: {uniqueWordCount}");
            }
            else
            {
                Debug.LogError($"Failed to update unique words: {task.Exception}");
            }
        });
    }
}