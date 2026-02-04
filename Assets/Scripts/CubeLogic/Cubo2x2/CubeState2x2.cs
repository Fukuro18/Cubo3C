using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeState2x2 : MonoBehaviour
{
    public List<GameObject> up = new();
    public List<GameObject> down = new();
    public List<GameObject> left = new();
    public List<GameObject> right = new();
    public List<GameObject> front = new();
    public List<GameObject> back = new();

    public static bool autoRotating = false;
    public static bool started = false;

    private bool isShuffling = false;
    private SelectFace2x2 selectFace;

    void Start()
    {
        selectFace = GetComponent<SelectFace2x2>();
        if (selectFace == null)
        {
            Debug.LogError("[CubeState2x2] No se encontro SelectFace2x2 en el mismo GameObject");
        }
    }

    void PickUp(List<GameObject> cubeSide)
    {
        Transform pivot = cubeSide[0].transform.parent;
        Transform root = transform;

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

    public void Shuffle(int moves = 20)
    {
        if (isShuffling)
        {
            Debug.LogWarning("[CubeState2x2] Ya se esta mezclando el cubo");
            return;
        }

        if (selectFace == null)
        {
            Debug.LogError("[CubeState2x2] No se puede mezclar - SelectFace2x2 no encontrado");
            return;
        }

        StartCoroutine(ShuffleCoroutine(moves));
    }

    private IEnumerator ShuffleCoroutine(int moves)
    {
        isShuffling = true;
        Debug.Log($"[CubeState2x2] Iniciando mezcla con {moves} movimientos");

        string[] faces = { "U", "D", "L", "R", "F", "B" };

        for (int i = 0; i < moves; i++)
        {
            while (selectFace.IsAnyFaceRotating())
            {
                yield return null;
            }

            string randomFace = faces[Random.Range(0, faces.Length)];
            
            Debug.Log($"[CubeState2x2] Mezcla {i + 1}/{moves}: Rotando cara {randomFace}");
            
            selectFace.RotateFaceByName(randomFace);

            yield return new WaitForSeconds(0.1f);
        }

        while (selectFace.IsAnyFaceRotating())
        {
            yield return null;
        }

        isShuffling = false;
        Debug.Log("[CubeState2x2] Mezcla completada");
    }

    public bool IsShuffling()
    {
        return isShuffling;
    }

    public void StopShuffle()
    {
        if (isShuffling)
        {
            StopAllCoroutines();
            isShuffling = false;
            Debug.Log("[CubeState2x2] Mezcla detenida");
        }
    }
}
