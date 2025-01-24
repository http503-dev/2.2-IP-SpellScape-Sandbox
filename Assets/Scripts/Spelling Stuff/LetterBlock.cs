/*
 * Author: Muhammad Farhan
 * Date: 22/1/2024
 * Description: Script that handle functionality of the letter blocks in the sandbox scene
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class LetterBlock : MonoBehaviour
{
    /// <summary>
    /// References to things that make it work
    /// </summary>
    public string letter; // The letter this block represents
    private XRSocketInteractor rightSocket; // The socket to the right of this block
    private WordDiscovery wordDiscovery; // Reference to the WordDiscovery system
    public LetterBlock connectedToLeft; // Tracks the block connected to the left

    /// <summary>
    /// Initializes the block by referencing the socket and the WordDiscovery system
    /// </summary>
    private void Awake()
    {
        // Cache the right socket interactor
        rightSocket = GetComponentInChildren<XRSocketInteractor>();

        if (rightSocket == null)
        {
            Debug.LogError($"No XRSocketInteractor found on {gameObject.name}");
        }

        // Find the WordDiscovery system in the scene
        wordDiscovery = FindObjectOfType<WordDiscovery>();

        if (wordDiscovery == null)
        {
            Debug.LogError("WordDiscovery system not found in the scene!");
        }
    }

    /// <summary>
    /// Registers the block's socket with the WordDiscovery system on start
    /// </summary>
    private void Start()
    {
        // Register this block's socket with the WordDiscovery system
        if (rightSocket != null && wordDiscovery != null)
        {
            wordDiscovery.RegisterSocket(rightSocket);
        }
    }

    /// <summary>
    /// Unregisters the block's socket from the WordDiscovery system when the block is destroyed
    /// </summary>
    private void OnDestroy()
    {
        // Unregister this block's socket when the block is destroyed
        if (rightSocket != null && wordDiscovery != null)
        {
            wordDiscovery.UnregisterSocket(rightSocket);
        }
    }

    /// <summary>
    /// Traverses backward through connected blocks to find the root block
    /// </summary>
    /// <returns>The root block (leftmost block in the chain)</returns>
    public LetterBlock GetRootBlock()
    {
        LetterBlock currentBlock = this;

        // Traverse backward through the connectedToLeft relationship
        while (currentBlock.connectedToLeft != null)
        {
            Debug.Log($"Traversing to parent block: {currentBlock.connectedToLeft.letter}");
            currentBlock = currentBlock.connectedToLeft;
        }

        // If no connectedToLeft relationship exists, this block is the root
        Debug.Log($"Root block: {currentBlock.letter}");
        return currentBlock;
    }

    /// <summary>
    /// Forms the word by traversing all connected blocks starting from this block.
    /// </summary>
    /// <returns>The word formed by connected blocks</returns>
    public string GetFormedWord()
    {
        string formedWord = letter;
        LetterBlock currentBlock = this;

        // Traverse through the chain of connected blocks via the right socket
        while (currentBlock.rightSocket != null && currentBlock.rightSocket.hasSelection)
        {
            var nextInteractable = currentBlock.rightSocket.GetOldestInteractableSelected();
            var nextBlock = nextInteractable?.transform.GetComponent<LetterBlock>();

            if (nextBlock != null)
            {
                formedWord += nextBlock.letter;
                currentBlock = nextBlock;
            }
            else
            {
                break;
            }
        }

        Debug.Log($"Formed word: {formedWord}");
        return formedWord;
    }
}
