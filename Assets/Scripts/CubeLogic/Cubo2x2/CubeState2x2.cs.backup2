using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeState2x2 : MonoBehaviour

{
    public List<GameObject> front = new();
    public List<GameObject> back = new();
    public List<GameObject> up = new();
    public List<GameObject> down = new();
    public List<GameObject> left = new();
    public List<GameObject> right = new();

    public static bool autoRotating = false;
    public static bool startedAutoRotate = false;
    public static bool started = false;


    public void PickUp(List<GameObject> cubeSide)
    {
        // pivot real de la cara (GameObject vacío)
        Transform pivot = cubeSide[0].transform.parent.parent;

        foreach (GameObject face in cubeSide)
        {
            Transform littleCube = face.transform.parent; // ?? EL CUBITO
            littleCube.SetParent(pivot, true);
        }
    }


    public void PutDown(List<GameObject> cubeSide, Transform root)
    {
        foreach (GameObject face in cubeSide)
        {
            Transform littleCube = face.transform.parent;
            littleCube.SetParent(root, true);
        }
    }


    string GetSideString(List<GameObject> side)
    {
        string sideString = "";

        foreach (GameObject face in side)
        {
            if (face != null)
                sideString += face.name[0];
            else
                sideString += "U";
        }

        return sideString;
    }

    public string GetStateString()
    {
        return
            GetSideString(up) +
            GetSideString(right) +
            GetSideString(front) +
            GetSideString(down) +
            GetSideString(left) +
            GetSideString(back);
    }
}
