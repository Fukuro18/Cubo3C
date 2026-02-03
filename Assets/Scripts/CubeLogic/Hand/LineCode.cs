using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCode : MonoBehaviour
{
    LineRenderer lineRenderer;

    public Transform origin;
    public Transform destination;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        // Validate that references are assigned before using them
        if (origin == null || destination == null)
        {
            Debug.LogWarning("LineCode: Origin or Destination not assigned in Inspector. Please assign both references.");
            return;
        }

        if (lineRenderer == null)
        {
            Debug.LogWarning("LineCode: LineRenderer component not found.");
            return;
        }

        lineRenderer.SetPosition(0, origin.position);
        lineRenderer.SetPosition(1, destination.position);
        lineRenderer.SetColors(Color.red, Color.green);
    }
}
