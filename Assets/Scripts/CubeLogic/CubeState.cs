using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeState : MonoBehaviour
{
    // sides
    public List<GameObject> front = new List<GameObject>();
    public List<GameObject> back = new List<GameObject>();
    public List<GameObject> up = new List<GameObject>();
    public List<GameObject> down = new List<GameObject>();
    public List<GameObject> left = new List<GameObject>();
    public List<GameObject> right = new List<GameObject>();

    public static bool autoRotating = false;
    public static bool startedAutoRotate = false;
    public static bool started = false;

    // Start is called before the first frame update
    void Start()
    {
      //  CubeState.started = true;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PickUp(List<GameObject> cubeSide)
    {
        
        foreach (GameObject face in cubeSide)
        {
          //  face.transform.parent.SetParent(pivot);
            // Attach the parent of each face (the little cube)
            // to the parent of the 4th index (the little cube in the middle)
            // Unless it is already the 4th index
             if (face != cubeSide[4])
              {
                  face.transform.parent.transform.parent = cubeSide[4].transform.parent;
                  //start the side rotation logic
              }
             //face.transform.parent.transform.parent = cubeSide[4].transform.parent;

        }

    }



    // Imagen a27e21.png: Función PutDown o UnParent (Complemento de PickUp)
    public void PutDown(List<GameObject> littleCubes, Transform pivot)
    {
        foreach (GameObject littleCube in littleCubes)
        {
            if (littleCube != littleCubes[4])
            {
                littleCube.transform.parent.transform.parent = pivot;
            }
        }
    }

    string GetSideString(List<GameObject> side)
    {
        string sideString = "";
        foreach (GameObject face in side)
        {
            if (face != null)
            {
                sideString += face.name[0].ToString();
            }
            else
            {
                sideString += "U"; // Placeholder for missing face to avoid crash, though solution will likely be wrong.
            }
        }
        return sideString;
    }

    public string GetStateString()
    {
        string stateString = "";
        stateString += GetSideString(up);
        stateString += GetSideString(right);
        stateString += GetSideString(front);
        stateString += GetSideString(down);
        stateString += GetSideString(left);
        stateString += GetSideString(back);
        return stateString;
    }

 
}
