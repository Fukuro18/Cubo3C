using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HandUIInteraction : MonoBehaviour
{
    [Header("References")]
    public HandInteractionManager interactionManager;
    public Camera mainCamera;

    [Header("UI Interaction Settings")]
    public float maxRayDistance = 100f;
    public LayerMask uiLayerMask;

    // Current state
    private GameObject currentHoveredButton;
    private bool wasClickTriggered = false;

    void Start()
    {
        if (interactionManager == null)
        {
            interactionManager = FindObjectOfType<HandInteractionManager>();
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Subscribe to pinch events
        if (interactionManager != null)
        {
            interactionManager.onPinchStart += OnPinchDetected;
            interactionManager.onPinchEnd += OnPinchReleased;
        }
    }

    void Update()
    {
        if (interactionManager == null || mainCamera == null) return;

        // Get pointer position from hand
        Vector3 pointerPosition = interactionManager.GetPointerPosition();

        // Raycast from camera through pointer position
        PerformUIRaycast(pointerPosition);
    }

    void PerformUIRaycast(Vector3 worldPointerPosition)
    {
        // Convert world position to screen position
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPointerPosition);

        // Create PointerEventData for UI raycast
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        // Raycast against UI
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        GameObject newHoveredButton = null;

        // Find the first valid UI button
        foreach (RaycastResult result in results)
        {
            Button button = result.gameObject.GetComponent<Button>();
            if (button != null && button.interactable)
            {
                newHoveredButton = result.gameObject;
                break;
            }

            // Also check parent for button component
            button = result.gameObject.GetComponentInParent<Button>();
            if (button != null && button.interactable)
            {
                newHoveredButton = button.gameObject;
                break;
            }
        }

        // Handle hover state changes
        if (newHoveredButton != currentHoveredButton)
        {
            // Exit previous button
            if (currentHoveredButton != null)
            {
                ExecuteEvents.Execute(currentHoveredButton, pointerData, ExecuteEvents.pointerExitHandler);
                interactionManager.SetInteracting(false);
            }

            // Enter new button
            if (newHoveredButton != null)
            {
                ExecuteEvents.Execute(newHoveredButton, pointerData, ExecuteEvents.pointerEnterHandler);
                interactionManager.SetInteracting(true);
                Debug.Log($"Hovering over button: {newHoveredButton.name}");
            }

            currentHoveredButton = newHoveredButton;
        }
    }

    void OnPinchDetected()
    {
        if (currentHoveredButton != null && !wasClickTriggered)
        {
            // Execute button click
            Button button = currentHoveredButton.GetComponent<Button>();
            if (button != null && button.interactable)
            {
                button.onClick.Invoke();
                wasClickTriggered = true;
                Debug.Log($"Button clicked by hand gesture: {currentHoveredButton.name}");

                // Visual feedback
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                ExecuteEvents.Execute(currentHoveredButton, pointerData, ExecuteEvents.pointerDownHandler);

                // Schedule pointer up event
                StartCoroutine(TriggerPointerUp());
            }
        }
    }

    void OnPinchReleased()
    {
        wasClickTriggered = false;
    }

    IEnumerator TriggerPointerUp()
    {
        yield return new WaitForSeconds(0.1f);

        if (currentHoveredButton != null)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            ExecuteEvents.Execute(currentHoveredButton, pointerData, ExecuteEvents.pointerUpHandler);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (interactionManager != null)
        {
            interactionManager.onPinchStart -= OnPinchDetected;
            interactionManager.onPinchEnd -= OnPinchReleased;
        }
    }

    // Visualize raycast in editor
    void OnDrawGizmos()
    {
        if (interactionManager != null && mainCamera != null)
        {
            Vector3 pointerPos = interactionManager.GetPointerPosition();
            Gizmos.color = currentHoveredButton != null ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(pointerPos, 0.05f);

            // Draw ray from camera to pointer
            Gizmos.DrawLine(mainCamera.transform.position, pointerPos);
        }
    }
}
