using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolveTwoPhase2x2 : MonoBehaviour
{
    public ReadCube2x2 readCube;
    public CubeState2x2 cubeState;
    public Automate2x2 automate;

    void Start()
    {
        readCube = FindObjectOfType<ReadCube2x2>();
        cubeState = FindObjectOfType<CubeState2x2>();
        automate = FindObjectOfType<Automate2x2>();
    }

    public void Solver()
    {
        if (readCube == null || cubeState == null || automate == null)
        {
            Debug.LogError("[SolveTwoPhase2x2] Faltan referencias (ReadCube2x2, CubeState2x2 o Automate2x2)");
            return;
        }

        readCube.ReadState();

        // Get the state of the cube as a string
        string moveString = cubeState.GetStateString();
        Debug.Log($"[SolveTwoPhase2x2] Estado del cubo: {moveString}");

        // 这里 would be the call to the 2x2 Algorithm.
        // Since we don't have Kociemba for 2x2, we will simplify:
        // TODO: Implement actual BFS solver or connect to external library.
        // For now, we will just demonstrate the FLOW by generating a random "solution" (Shuffle).
        
        Debug.LogWarning("[SolveTwoPhase2x2] 2x2 Solver implementation pending. Generating random moves for demo.");
        
        // Simulating a solution string "U F' R2 B"
        string fakeSolution = GenerateRandomSolution(); 
        
        List<string> solutionList = StringToList(fakeSolution);

        // Automate the list
        automate.movelist = solutionList;
    }

    string GenerateRandomSolution()
    {
        // Generates 5 random moves to simulate a "solution"
        string[] moves = { "U", "U'", "R", "R'", "F", "F'", "U2", "R2", "F2" };
        List<string> result = new List<string>();
        for(int i=0; i<5; i++)
        {
            result.Add(moves[Random.Range(0, moves.Length)]);
        }
        return string.Join(" ", result);
    }

    List<string> StringToList(string solution)
    {
        return new List<string>(solution.Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries));
    }
}
