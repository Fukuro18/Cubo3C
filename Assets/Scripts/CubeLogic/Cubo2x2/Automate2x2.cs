using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Automate2x2 : MonoBehaviour
{
    public List<string> movelist = new List<string>();
    private readonly List<string> allMoves = new List<string>() 
    { 
        "U", "U'", "U2", 
        "D", "D'", "D2", 
        "L", "L'", "L2", 
        "R", "R'", "R2", 
        "F", "F'", "F2", 
        "B", "B'", "B2" 
    };

    private SelectFace2x2 selectFace;
    private ReadCube2x2 readCube;

    void Start()
    {
        selectFace = FindObjectOfType<SelectFace2x2>();
        readCube = FindObjectOfType<ReadCube2x2>();
    }

    void Update()
    {
        // Execute moves if list has items and cube is not currently rotating
        if (movelist.Count > 0 && !selectFace.IsAnyFaceRotating())
        {
            DoMove(movelist[0]);
            movelist.RemoveAt(0);
        }
    }

    public void Shuffle()
    {
        List<string> moves = new List<string>();
        int shuffleLength = Random.Range(10, 20); // 10-20 moves for 2x2
        for (int i = 0; i < shuffleLength; i++)
        {
            int randomMove = Random.Range(0, allMoves.Count);
            moves.Add(allMoves[randomMove]);
        }
        movelist = moves;
    }

    void DoMove(string move)
    {
        if (readCube != null) readCube.ReadState(); // Optional: Update state before move if needed

        string face = move.Substring(0, 1);
        float angle = 90f;

        if (move.Length > 1)
        {
            if (move[1] == '\'')
            {
                angle = -90f;
            }
            else if (move[1] == '2')
            {
                angle = 180f;
            }
        }

        Debug.Log($"[Automate2x2] Ejecutando movimiento: {move} (Cara: {face}, Ángulo: {angle})");
        selectFace.RotateFaceByName(face, angle);
    }
}
