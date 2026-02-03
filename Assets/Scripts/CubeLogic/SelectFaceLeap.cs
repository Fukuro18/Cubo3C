using System.Collections.Generic;
using UnityEngine;

public class SelectFaceLeap : MonoBehaviour

{
    CubeState cubeState;
    ReadCube readCube;

    public float dragThreshold = 0.02f;

    private Vector3 fingerStartPos;
    private Collider finger;
    private bool dragging = false;

    void Start()
    {
        cubeState = FindObjectOfType<CubeState>();
        readCube = FindObjectOfType<ReadCube>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Finger")) return;

        finger = other;
        fingerStartPos = other.transform.position;
        dragging = false;
    }

    void OnTriggerExit(Collider other)
    {
        if (other == finger)
        {
            finger = null;
            dragging = false;
        }
    }

    void Update()
    {
        if (finger == null || dragging) return;

        Vector3 delta = finger.transform.position - fingerStartPos;

        if (delta.magnitude > dragThreshold)
        {
            readCube.ReadState();
            dragging = true;
            AttemptPickup(gameObject, delta);
        }
    }

    void AttemptPickup(GameObject face, Vector3 drag)
    {
        List<List<GameObject>> sides = new List<List<GameObject>> {
            cubeState.up, cubeState.down,
            cubeState.left, cubeState.right,
            cubeState.front, cubeState.back
        };

        List<GameObject> side = null;
        foreach (var s in sides)
            if (s.Contains(face)) side = s;

        if (side == null) return;

        Vector3 localX = face.transform.right;
        Vector3 localY = face.transform.up;

        bool column = Mathf.Abs(Vector3.Dot(drag.normalized, localY)) >
                      Mathf.Abs(Vector3.Dot(drag.normalized, localX));

        Vector3 desiredAxis = column ? localX : localY;

        foreach (var s in sides)
        {
            Vector3 axis = (s[4].transform.parent.position - readCube.transform.position).normalized;

            if (Mathf.Abs(Vector3.Dot(axis, desiredAxis)) > 0.5f)
            {
                cubeState.PickUp(s);
                s[4].transform.parent.GetComponent<PivotRotation>().Rotate(s);
                break;
            }
        }
    }
}


/*
{
CubeState cubeState;
ReadCube readCube;

// Static flag so RotateBigCubeLeap knows we are busy
public static bool IsActive = false;

public float dragThreshold = 0.03f;
public float rayDistance = 0.15f;

private bool isDragging = false;
private Vector3 fingerStartPos;
private GameObject currentFace;
private Collider currentFinger;

int layerMask = 1 << 8; // Caras del cubo

void Start()
{
    readCube = FindObjectOfType<ReadCube>();
    cubeState = FindObjectOfType<CubeState>();
}

void OnTriggerEnter(Collider other)
{
    if (!other.CompareTag("Finger")) return;

    currentFinger = other;
    fingerStartPos = other.transform.position;
    isDragging = false;

    // 🔥 Raycast para obtener la CARA REAL
    RaycastHit hit;

    // Raycast from finger
    if (Physics.Raycast(
        other.transform.position,
        other.transform.forward, 
        out hit,
        rayDistance,
        layerMask))
    {
        currentFace = hit.collider.gameObject;
        IsActive = true; // CLAIM interaction
    }
    else
    {
        currentFace = null;
        IsActive = false;
    }
}

void OnTriggerExit(Collider other)
{
    if (other == currentFinger)
    {
        isDragging = false;
        currentFace = null;
        currentFinger = null;
        IsActive = false; // RELEASE interaction
    }
}

void Update()
{
    if (CubeState.autoRotating) return;
    if (currentFace == null || currentFinger == null) {
         IsActive = false;
         return;
    }

    IsActive = true; // Ensure it stays true while hovering

    Vector3 fingerDelta = currentFinger.transform.position - fingerStartPos;

    if (!isDragging && fingerDelta.magnitude > dragThreshold)
    {
        PivotRotation[] pivots = FindObjectsOfType<PivotRotation>();
        foreach (var p in pivots)
        {
            if (p.IsRotating()) return;
        }

        readCube.ReadState();
        isDragging = true;

        // FIX: Convert world drag vector to FACE LOCAL space
        // This ensures "up" is always relative to the face, not the world
        Vector3 localDrag = currentFace.transform.InverseTransformDirection(fingerDelta);

        // Now we use x and y from the local drag
        AttemptPickup(currentFace, localDrag);
    }
}


public void AttemptPickup(GameObject face, Vector3 dragVectorLocal)
{
    List<List<GameObject>> allSides = new List<List<GameObject>>()
    {
        cubeState.up,
        cubeState.down,
        cubeState.left,
        cubeState.right,
        cubeState.front,
        cubeState.back
    };

    List<GameObject> currentSide = null;

    foreach (var side in allSides)
    {
        if (side.Contains(face))
        {
            currentSide = side;
            break;
        }
    }

    if (currentSide == null) return;

    // Because we are now using LOCAL drag vector relative to the face:
    // x component corresponds to local Right
    // y component corresponds to local Up

    // Direction from local vector
    Vector2 dragDir = new Vector2(dragVectorLocal.x, dragVectorLocal.y).normalized;

    // Pure local definitions
    Vector2 xDir = Vector2.right; // (1,0)
    Vector2 yDir = Vector2.up;    // (0,1)

    bool isColDrag =
        Mathf.Abs(Vector2.Dot(dragDir, yDir)) >
        Mathf.Abs(Vector2.Dot(dragDir, xDir));

    // In local space, Right (X) is the axis for Column rotation (around X axis of face)
    // Up (Y) is the axis for Row rotation (around Y axis of face... wait)

    // Let's re-verify:
    // If I drag UP (Y), I want to rotate the column. Area of effect is vertical. 
    // Example: Front Face. Drag Up. Should rotate Left/Right center slices? No, Front Face Up drag rotates Left or Right side.
    // Actually, dragging UP on Front Face usually means rotating the side slices (Left/Right) around the X axis.
    // Wait, standard behavior:
    // Drag Vertical (Y) -> Rotate around X axis (Horizontal Axis)
    // Drag Horizontal (X) -> Rotate around Y axis (Vertical Axis)

    Vector3 desiredAxis;
    if (isColDrag) // Vertical Drag (Y)
    {
         desiredAxis = face.transform.right; // Rotate around Local X
    }
    else // Horizontal Drag (X)
    {
         desiredAxis = face.transform.up;    // Rotate around Local Y
    }

    foreach (var side in allSides)
    {
        Vector3 sideAxis =
            (side[4].transform.parent.position - readCube.transform.position).normalized;

        if (Mathf.Abs(Vector3.Dot(sideAxis, desiredAxis)) > 0.5f)
        {
            cubeState.PickUp(side);
            Transform pivot = cubeState.GetPivot(side);
            pivot.GetComponent<PivotRotation>().Rotate(side);
        }
    }
}
}*/
