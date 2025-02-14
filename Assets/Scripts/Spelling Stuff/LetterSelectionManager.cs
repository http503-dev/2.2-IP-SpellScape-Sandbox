/*
 * Author: Muhammad Farhan
 * Date: 14/2/2024
 * Description: Manager handling image block interactions
 */
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class LetterSelectionManager : MonoBehaviour
{
    /// <summary>
    /// Reference to the image block component found in the letter blocks
    /// </summary>
    private ImageBlock selectedImageBlock;

    /// <summary>
    /// Function to select the image block
    /// </summary>
    /// <param name="block"> The image block being selected </param>
    public void SelectImageBlock(ImageBlock block)
    {
        selectedImageBlock = block;
        Debug.Log($"Selected Letter: {block.letter}");
    }

    /// <summary>
    /// Function to place the letter block in the slot
    /// </summary>
    /// <param name="slot"> The slot the letter block is being placed in </param>
    public void PlaceLetterInSlot(WordSlot slot)
    {
        if (selectedImageBlock != null)
        {
            slot.SetLetter(selectedImageBlock.letter);

            // Move block to the slot's position
            selectedImageBlock.transform.position = slot.transform.position;
            selectedImageBlock.transform.rotation = slot.transform.rotation;

            // Disable grab interactable after placing
            var grabInteractable = selectedImageBlock.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null)
            {
                grabInteractable.enabled = false;
            }

            // Clear selection
            selectedImageBlock = null;

            // Check for word completion after placing
            if (slot.transform.parent.TryGetComponent<WordValidator>(out WordValidator validator))
            {
                CheckForWordCompletion(validator);
            }
            else
            {
                Debug.LogWarning("WordValidator not found on slot's parent!");
            }
        }
        else
        {
            Debug.Log("No letter selected to place.");
        }
    }

    /// <summary>
    /// Function to call for validation once all slots are filled
    /// </summary>
    /// <param name="validator"></param>
    private void CheckForWordCompletion(WordValidator validator)
    {
        WordSlot[] slots = validator.GetComponentsInChildren<WordSlot>();

        // Check if all slots are filled (i.e., no null values)
        if (slots.All(s => s.storedLetter.HasValue))
        {
            string formedWord = string.Concat(slots.Select(s => s.storedLetter.Value));
            Debug.Log($"Formed Word: {formedWord}");

            if (formedWord.Length == validator.correctWord.Length)
            {
                validator.ValidateCustomWord(formedWord);
            }
        }
        else
        {
            Debug.Log("Not all slots are filled yet.");
        }
    }

}
