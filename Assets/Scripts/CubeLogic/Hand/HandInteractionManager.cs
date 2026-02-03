using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteractionManager : MonoBehaviour
{
    [Header("Hand Tracking Reference")]
    public HandTracking handTracking;

    [Header("Gesture Detection Settings")]
    [Header("Gesture Detection Settings")]
    public float pinchThreshold = 0.5f; // Increased from 0.1 to 0.5 for easier clicking
    public float pinchHoldTime = 0.1f;  // Decreased from 0.2 to 0.1 for snappier clicks
    public float palmOpenThreshold = 0.7f; // Adjusted relative to pinch threshold

    [Header("Interaction States")]
    public bool isPinching = false;
    public bool isPalmOpen = false;
    public bool isInteracting = false;

    [Header("Visual Feedback")]
    public GameObject virtualPointer;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;
    public Color clickColor = Color.green;

    // Private state tracking
    private float pinchStartTime = 0f;
    private bool pinchHeld = false;
    private Vector3 lastPalmPosition;
    private Vector3 palmVelocity;

    // Events for other systems
    public delegate void OnPinchStart();
    public delegate void OnPinchEnd();
    public delegate void OnPalmMove(Vector3 velocity);

    public event OnPinchStart onPinchStart;
    public event OnPinchEnd onPinchEnd;
    public event OnPalmMove onPalmMove;

    void Start()
    {
        if (handTracking == null)
        {
            handTracking = FindObjectOfType<HandTracking>();
            if (handTracking == null)
            {
                Debug.LogError("HandInteractionManager: No HandTracking found in scene!");
                return; // Exit early if no HandTracking found
            }
        }

        // Create virtual pointer if needed
        if (virtualPointer == null)
        {
            CreateVirtualPointer();
        }

        // Initialize palm position only if handTracking is valid
        if (handTracking != null)
        {
            lastPalmPosition = handTracking.GetPalmPosition();
        }
    }

    void Update()
    {
        if (handTracking == null || !handTracking.IsHandDetected())
        {
            ResetGestures();
            return;
        }

        // Update gesture detection
        DetectPinchGesture();
        DetectPalmState();
        UpdateVirtualPointer();
        CalculatePalmVelocity();
    }

    void DetectPinchGesture()
    {
        float pinchDistance = handTracking.GetPinchDistance();

        if (pinchDistance < pinchThreshold)
        {
            if (!isPinching)
            {
                // Pinch just started
                isPinching = true;
                pinchStartTime = Time.time;
                pinchHeld = false;
            }
            else if (!pinchHeld && (Time.time - pinchStartTime) >= pinchHoldTime)
            {
                // Pinch held long enough - trigger event
                pinchHeld = true;
                onPinchStart?.Invoke();
                Debug.Log("Pinch gesture detected!");
            }
        }
        else
        {
            if (isPinching && pinchHeld)
            {
                // Pinch released
                onPinchEnd?.Invoke();
                Debug.Log("Pinch released!");
            }

            isPinching = false;
            pinchHeld = false;
        }
    }

    void DetectPalmState()
    {
        // Simple palm detection - can be enhanced later
        float pinchDistance = handTracking.GetPinchDistance();
        isPalmOpen = pinchDistance > palmOpenThreshold;
    }

    void CalculatePalmVelocity()
    {
        Vector3 currentPalmPos = handTracking.GetPalmPosition();
        palmVelocity = (currentPalmPos - lastPalmPosition) / Time.deltaTime;
        lastPalmPosition = currentPalmPos;

        if (isPalmOpen && palmVelocity.magnitude > 0.1f)
        {
            onPalmMove?.Invoke(palmVelocity);
        }
    }

    void UpdateVirtualPointer()
    {
        if (virtualPointer == null) return;

        // Position the pointer at the index finger tip
        virtualPointer.transform.position = handTracking.GetIndexTipPosition();

        // Update pointer color based on state
        Renderer renderer = virtualPointer.GetComponent<Renderer>();
        if (renderer != null)
        {
            if (pinchHeld)
            {
                renderer.material.color = clickColor;
            }
            else if (isInteracting)
            {
                renderer.material.color = hoverColor;
            }
            else
            {
                renderer.material.color = normalColor;
            }
        }
    }

    void CreateVirtualPointer()
    {
        virtualPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        virtualPointer.name = "VirtualPointer";
        virtualPointer.transform.localScale = Vector3.one * 0.05f;
        virtualPointer.transform.SetParent(transform);

        // Make it non-physical
        Collider collider = virtualPointer.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }

        // Add material
        Renderer renderer = virtualPointer.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = new Material(Shader.Find("Standard"));
            renderer.material.color = normalColor;
        }
    }

    void ResetGestures()
    {
        if (isPinching && pinchHeld)
        {
            onPinchEnd?.Invoke();
        }

        isPinching = false;
        pinchHeld = false;
        isPalmOpen = false;
        isInteracting = false;
    }

    // Public API for other scripts
    public bool IsPinching()
    {
        return isPinching && pinchHeld;
    }

    public bool IsPalmOpen()
    {
        return isPalmOpen;
    }

    public Vector3 GetPointerPosition()
    {
        return handTracking.GetIndexTipPosition();
    }

    public Vector3 GetPalmVelocity()
    {
        return palmVelocity;
    }

    public void SetInteracting(bool state)
    {
        isInteracting = state;
    }

    public bool IsHandDetected()
    {
        return handTracking != null && handTracking.IsHandDetected();
    }
}
