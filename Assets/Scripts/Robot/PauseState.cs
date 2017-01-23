using UnityEngine;
using System.Collections;
using System;

public class PauseState : IRobotState
{
    public GameObject robot;
    private RobotBehaviour robotBehaviour;


    public PauseState(GameObject r)
    {
        robot = r;
    }
    public void EnterPauseState()
    {
        Debug.Log("already in pause");
    }

    public void EnterPlayState()
    {
        robotBehaviour.currentState = robot.GetComponent<RobotBehaviour>().playState;
    }

    public void UpdateState()
    {
        
    }
}
