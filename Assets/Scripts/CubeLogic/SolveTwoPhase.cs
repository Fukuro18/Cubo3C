using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kociemba;

public class SolveTwoPhase : MonoBehaviour
{

    public ReadCube readCube;
    public CubeState cubeState;
    public Automate automate;

    // Start is called before the first frame update
    void Start()
    {
        readCube = FindObjectOfType<ReadCube>();
        cubeState = FindObjectOfType<CubeState>();
        automate = FindObjectOfType<Automate>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void Solver()
    {
        readCube.ReadState();

        // get the state of the cube as a string
        string moveString = cubeState.GetStateString();
        print(moveString);

        if (moveString.Length != 54)
        {
            Debug.LogError("Solver: Invalid move string length: " + moveString.Length);
            return;
        }

        // solve the cube
        string info = "";
        string solution = "";

        try
        {
            // First time build the tables
            solution = SearchRunTime.solution(moveString, out info, buildTables: true);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Solver: Exception during Kociemba solve: " + e.Message);
            return;
        }

        if (solution.Contains("Error"))
        {
            Debug.LogError("Solver: Kociemba returned error: " + solution);
            return;
        }

        //Every other time
        //string solution = Search.solution(moveString, out info);

        // convert the solved moves from a string to a list
        List<string> solutionList = StringToList(solution);

        //Automate the list
        automate.movelist = solutionList;

        print(info);

    }

    List<string> StringToList(string solution)
    {
        List<string> solutionList = new List<string>(solution.Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries));
        return solutionList;
    }
}
