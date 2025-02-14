/*
 * Author: Muhammad Farhan
 * Date: 14/2/2024
 * Description: Script for the letter blocks used in the image tracking minigame
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ImageBlock : MonoBehaviour
{
    /// <summary>
    /// The character of the block
    /// </summary>
    public char letter;

    /// <summary>
    /// References to the selection manager and various location based variables
    /// </summary>
    private LetterSelectionManager selectionManager;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Transform initialParent;

    /// <summary>
    /// Called on start to save the block's position and the selection manager
    /// </summary>
    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        initialParent = transform.parent;
        selectionManager = FindObjectOfType<LetterSelectionManager>();
    }

    /// <summary>
    /// Function to save the block for when a block is selected
    /// </summary>
    public void OnBlockSelected()
    {
        if (selectionManager != null)
        {
            selectionManager.SelectImageBlock(this);
            Debug.Log($"Letter {letter} selected!");
        }
    }

    /// <summary>
    /// Function to call the reset coroutine
    /// </summary>
    public void ResetBlock()
    {
        StartCoroutine(SmoothMove(initialPosition, initialRotation, initialParent));
    }

    /// <summary>
    /// Coroutine for resetting the blocks to their previous locations
    /// </summary>
    /// <param name="targetPosition"> The target location </param>
    /// <param name="targetRotation"> The target rotation </param>
    /// <param name="targetParent"> The target parent </param>
    /// <returns></returns>
    private IEnumerator SmoothMove(Vector3 targetPosition, Quaternion targetRotation, Transform targetParent)
    {
        float duration = 0.5f; // Smooth move duration
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float time = 0;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPosition, time / duration);
            transform.rotation = Quaternion.Lerp(startRot, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;

        transform.SetParent(targetParent); // Go back to initial parent

        var grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.enabled = true;
        }
    }
}
