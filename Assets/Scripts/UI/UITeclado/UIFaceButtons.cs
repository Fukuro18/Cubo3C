using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFaceButtons : MonoBehaviour
{
    // Start is called before the first frame update

    public CubeState cubeState;
    public ReadCube readCube;

    private PivotRotation pivot;

    void Start()
    {
        cubeState = FindObjectOfType<CubeState>();
        readCube = FindObjectOfType<ReadCube>();
        pivot = readCube.GetComponentInChildren<PivotRotation>();
    }

    // ---------- BOTONES ----------

    public void RotateUp()
    {
        RotateFace(cubeState.up, 90f);
    }

    public void RotateDown()
    {
        RotateFace(cubeState.down, -90f);
    }

    public void RotateLeft()
    {
        RotateFace(cubeState.left, 90f);
    }

    public void RotateRight()
    {
        RotateFace(cubeState.right, -90f);
    }

    public void RotateFront()
    {
        RotateFace(cubeState.front, 90f);
    }

    public void RotateBack()
    {
        RotateFace(cubeState.back, -90f);
    }

    // ---------- CORE ----------
    void RotateFace(List<GameObject> side, float angle)
    {
        if (PivotRotationIsBusy()) return;

        readCube.ReadState();
        cubeState.PickUp(side);

        PivotRotation pr = side[4].transform.parent.GetComponent<PivotRotation>();
        pr.StartAutoRotate(side, angle);
    }

    bool PivotRotationIsBusy()
    {
        foreach (PivotRotation p in FindObjectsOfType<PivotRotation>())
        {
            if (p.IsRotating())
                return true;
        }
        return false;
    }
}


