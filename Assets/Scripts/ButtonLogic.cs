using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLogic : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Graph graph;

    [Header("Booleans")]
    private bool canSetWalls = false;
    private bool canSetStartAndGoal = false;
    private bool startPathfinder = false;

    //Properties
    public bool CanSetWalls
    {
        get
        {
            return canSetWalls;
        }
    }
    public bool StartPathfinder
    {
        get
        {
            return startPathfinder;
        }
        set
        {
            startPathfinder = value;
        }
    }

    public void EnableWallPlacing()
    {
        if (canSetStartAndGoal == false && startPathfinder == false)
        {
            canSetWalls = !canSetWalls;
            graph.CanSetWalls = canSetWalls;
        }
    }
    public void EnableStartAndGoalPlacing()
    {
        if (canSetWalls == false && startPathfinder == false)
        {
            canSetStartAndGoal = !canSetStartAndGoal;
            graph.CanSetStartAndGoal = canSetStartAndGoal;
        }
    }
    public void EnablePathfinder()
    {
        if (canSetWalls == false && canSetStartAndGoal == false)
        {
            startPathfinder = !startPathfinder;
            graph.StartPathfinder = startPathfinder;
        }
    }

}
