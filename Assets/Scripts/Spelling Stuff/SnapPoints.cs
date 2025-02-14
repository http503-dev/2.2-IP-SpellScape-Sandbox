using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;

public class SnapPoints : MonoBehaviour
{
    public int snapIndex;
    public WordValidator validator;

    private void Awake()
    {
        var interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnBlockPlaced);
            interactable.selectExited.AddListener(OnBlockRemoved);
        }
        else
        {
            Debug.LogWarning($"SnapPoint on {gameObject.name} is missing XRSimpleInteractable.");
        }
    }

    private void OnBlockPlaced(SelectEnterEventArgs args)
    {
        XRGrabInteractable block = args.interactableObject as XRGrabInteractable;
        if (block != null)
        {
            validator.PlaceBlock(block.gameObject, snapIndex);
        }
    }

    private void OnBlockRemoved(SelectExitEventArgs args)
    {
        validator.RemoveBlock(snapIndex);
    }
}
