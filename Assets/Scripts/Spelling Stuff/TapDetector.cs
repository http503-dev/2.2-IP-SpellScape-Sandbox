using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TapDetector : MonoBehaviour
{
    public Camera arCamera; // Assign your AR Camera

    public InputAction tapAction;

    private void OnEnable()
    {
        tapAction.Enable();
    }

    private void OnDisable()
    {
        tapAction.Disable();
    }

    private void Update()
    {
        if (tapAction.WasPressedThisFrame())
        {
            Ray ray = arCamera.ScreenPointToRay(Pointer.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log($"Tapped on: {hit.collider.gameObject.name}");
                // Handle block/snap point detection here
            }
        }
    }
}
