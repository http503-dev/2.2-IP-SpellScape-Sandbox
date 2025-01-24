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

public class WordDiscovery : MonoBehaviour
{
    /// <summary>
    /// A list to track all registered XRSocketInteractors in the scene
    /// </summary>
    private List<XRSocketInteractor> registeredSockets = new List<XRSocketInteractor>();

    /// <summary>
    /// Automatically registers any existing sockets in the scene
    /// </summary>
    private void OnEnable()
    {
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
    /// Validates the word against the dictionary
    /// </summary>
    private void ValidateWord(string word)
    {
        if (WordDictionaryLoader.Instance.IsValidWord(word))
        {
            Debug.Log($"Valid word formed: {word}");
        }
        else
        {
            Debug.Log($"Invalid word: {word}");
        }
    }
}
