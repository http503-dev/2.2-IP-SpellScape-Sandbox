/*
 * Author: Muhammad Farhan
 * Date: 14/2/2024
 * Description: Script for the word slots used in the image tracking minigame
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WordSlot : MonoBehaviour
{
    /// <summary>
    /// Int to tell where exactly the slot/letter is in correlation to the word
    /// </summary>
    public int slotIndex;

    /// <summary>
    /// Variable to store the char of the letter blocks
    /// </summary>
    public char? storedLetter = null;

    /// <summary>
    /// Reference to the selection manager
    /// </summary>
    private LetterSelectionManager selectionManager;

    /// <summary>
    /// Called on start to find the selection manager
    /// </summary>
    private void Start()
    {
        selectionManager = FindObjectOfType<LetterSelectionManager>();
    }

    /// <summary>
    /// Function to place a block into a slot
    /// </summary>
    public void OnSlotTapped()
    {
        if (selectionManager != null)
        {
            selectionManager.PlaceLetterInSlot(this);
        }
    }

    /// <summary>
    /// Function to set the char of the letter block
    /// </summary>
    /// <param name="letter"> The char of the letter block </param>
    public void SetLetter(char letter)
    {
        storedLetter = letter;
        Debug.Log($"Slot {slotIndex} filled with {letter}");
    }

    /// <summary>
    /// Function to clear the slot on reset
    /// </summary>
    public void ClearSlot()
    {
        storedLetter = null;
    }
}
