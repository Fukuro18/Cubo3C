using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class HandTracking : MonoBehaviour
{
    public UDPReceive udpReceive;
    public GameObject[] handPoints;

    [Header("Key Hand Points - Auto-assigned")]
    public Transform indexTip;      // Point 8: Index finger tip
    public Transform thumbTip;      // Point 4: Thumb tip
    public Transform palmCenter;    // Point 0: Wrist/Palm center

    [Header("Interaction Settings")]
    public bool addColliders = true;
    public float colliderRadius = 0.05f;
    public LayerMask handLayer;

    private bool collidersInitialized = false;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize colliders on first start
        if (addColliders && !collidersInitialized)
        {
            InitializeColliders();
        }

        // Auto-assign key hand points
        AssignKeyPoints();
    }

    // Update is called once per frame
    void Update()
    {
        if (string.IsNullOrEmpty(udpReceive.data)) return;

        string data = udpReceive.data;
        data = data.Remove(0, 1);
        data = data.Remove(data.Length - 1, 1);
        string[] points = data.Split(',');

        // Ensure we have enough data
        if (points.Length < 63) return; // 21 points * 3 coordinates = 63

        for (int i = 0; i < 21; i++)
        {
            if (handPoints[i] == null) continue;

            try
            {
                float x = 10 - float.Parse(points[i * 3], CultureInfo.InvariantCulture) / 90f;
                float y = float.Parse(points[i * 3 + 1], CultureInfo.InvariantCulture) / 90f;
                float z = float.Parse(points[i * 3 + 2], CultureInfo.InvariantCulture) / 90f;

                handPoints[i].transform.localPosition = new Vector3(x, y, z);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Error parsing hand point {i}: {e.Message}");
            }
        }
    }

    void InitializeColliders()
    {
        foreach (GameObject point in handPoints)
        {
            if (point == null) continue;

            // Add SphereCollider if it doesn't have one
            SphereCollider collider = point.GetComponent<SphereCollider>();
            if (collider == null)
            {
                collider = point.AddComponent<SphereCollider>();
            }

            collider.radius = colliderRadius;
            collider.isTrigger = true; // Use trigger for non-physical detection

            // Set layer if specified
            if (handLayer != 0)
            {
                point.layer = (int)Mathf.Log(handLayer.value, 2);
            }
        }

        collidersInitialized = true;
        Debug.Log("Hand colliders initialized successfully");
    }

    void AssignKeyPoints()
    {
        if (handPoints.Length >= 21)
        {
            palmCenter = handPoints[0].transform;  // Wrist
            thumbTip = handPoints[4].transform;    // Thumb tip
            indexTip = handPoints[8].transform;    // Index finger tip
        }
        else
        {
            Debug.LogError("HandTracking: Not enough hand points in array!");
        }
    }

    // Public getters for external scripts
    public Vector3 GetIndexTipPosition()
    {
        return indexTip != null ? indexTip.position : Vector3.zero;
    }

    public Vector3 GetThumbTipPosition()
    {
        return thumbTip != null ? thumbTip.position : Vector3.zero;
    }

    public Vector3 GetPalmPosition()
    {
        return palmCenter != null ? palmCenter.position : Vector3.zero;
    }

    public float GetPinchDistance()
    {
        if (indexTip != null && thumbTip != null)
        {
            return Vector3.Distance(indexTip.position, thumbTip.position);
        }
        return float.MaxValue;
    }

    public bool IsHandDetected()
    {
        return !string.IsNullOrEmpty(udpReceive.data);
    }
}
