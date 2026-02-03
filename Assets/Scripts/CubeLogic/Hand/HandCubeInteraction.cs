using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCubeInteraction : MonoBehaviour
{
    [Header("References")]
    public HandInteractionManager interactionManager;
    public SelectFace selectFace;
    public Camera mainCamera;

    [Header("Cube Interaction Settings")]
    public float proximityThreshold = 0.8f; // Distance to trigger interaction
    public float dragThreshold = 0.3f; // Minimum velocity to start drag
    public float dragSensitivity = 100f; // Multiplier for drag vector
    public LayerMask cubeLayerMask;

    // Private state
    private bool isDragging = false;
    private GameObject targetFace;
    private Vector3 dragStartPosition;
    private Vector3 lastPalmPosition;
    private float lastDragTime;

    void Start()
    {
        if (interactionManager == null)
        {
            interactionManager = FindObjectOfType<HandInteractionManager>();
        }

        if (selectFace == null)
        {
            selectFace = FindObjectOfType<SelectFace>();
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (interactionManager == null || !interactionManager.IsHandDetected())
        {
            if (isDragging)
            {
                EndDrag();
            }
            return;
        }

        // Get palm position
        Vector3 palmPosition = interactionManager.GetPointerPosition();

        // Check if palm is open (required for cube interaction)
        if (interactionManager.IsPalmOpen())
        {
            DetectCubeProximity(palmPosition);

            if (targetFace != null)
            {
                Vector3 palmVelocity = interactionManager.GetPalmVelocity();

                if (!isDragging && palmVelocity.magnitude > dragThreshold)
                {
                    StartDrag(palmPosition, palmVelocity);
                }
            }
        }
        else
        {
            if (isDragging)
            {
                EndDrag();
            }
            targetFace = null;
        }
    }

    void DetectCubeProximity(Vector3 palmPosition)
    {
        // Raycast from palm position towards cube
        RaycastHit hit;
        Vector3 rayDirection = mainCamera.transform.forward;

        // Also try direct raycast from camera through palm
        Ray ray = mainCamera.ScreenPointToRay(mainCamera.WorldToScreenPoint(palmPosition));

        if (Physics.Raycast(ray, out hit, proximityThreshold * 3f, cubeLayerMask))
        {
            // Check if it's a cube face
            if (hit.collider.gameObject.layer == 8) // Cube layer
            {
                targetFace = hit.collider.gameObject;
                Debug.Log($"Detected cube face: {targetFace.name}");
            }
        }
        else
        {
            targetFace = null;
        }
    }

    void StartDrag(Vector3 startPosition, Vector3 velocity)
    {
        if (targetFace == null) return;

        // Prevent multiple drags in quick succession
        if (Time.time - lastDragTime < 0.5f) return;

        isDragging = true;
        dragStartPosition = startPosition;
        lastPalmPosition = startPosition;
        lastDragTime = Time.time;

        Debug.Log($"Starting drag on face: {targetFace.name}");

        // Calculate drag direction in screen space
        Vector3 screenVelocity = CalculateScreenSpaceDrag(velocity);

        // Simulate the drag using SelectFace logic
        SimulateDragOnFace(targetFace, screenVelocity);
    }

    void EndDrag()
    {
        isDragging = false;
        targetFace = null;
        Debug.Log("Drag ended");
    }

    Vector3 CalculateScreenSpaceDrag(Vector3 worldVelocity)
    {
        // Convert world space velocity to screen space
        Vector3 palmScreenPos = mainCamera.WorldToScreenPoint(lastPalmPosition);
        Vector3 futureWorldPos = lastPalmPosition + worldVelocity * 0.1f;
        Vector3 futureScreenPos = mainCamera.WorldToScreenPoint(futureWorldPos);

        Vector3 screenDrag = (futureScreenPos - palmScreenPos) * dragSensitivity;
        return screenDrag;
    }

    void SimulateDragOnFace(GameObject face, Vector3 dragVector)
    {
        if (selectFace != null)
        {
            // Directly call the public AttemptPickup method
            selectFace.AttemptPickup(face, dragVector);
            Debug.Log($"Cube rotation triggered with drag vector: {dragVector}");
        }
        else
        {
            Debug.LogError("SelectFace reference is null");
        }

        // End the drag immediately after triggering rotation
        isDragging = false;
    }

    // Visualize in editor
    void OnDrawGizmos()
    {
        if (interactionManager != null && interactionManager.IsHandDetected())
        {
            Vector3 palmPos = interactionManager.GetPointerPosition();

            // Draw proximity sphere
            Gizmos.color = targetFace != null ? Color.green : Color.cyan;
            Gizmos.DrawWireSphere(palmPos, proximityThreshold);

            // Draw target face
            if (targetFace != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(palmPos, targetFace.transform.position);
            }
        }
    }
}
