using UnityEngine;
using System.Collections;
using System;

public class PauseState : IRobotState
{
    public GameObject robot;
    private RobotBehaviour robotScript;
   

    public PauseState(GameObject r)
    {
        robot = r;
        robotScript = robot.GetComponent<RobotBehaviour>();
    }
    public void EnterPauseState()
    {
        Debug.Log("already in pause");
    }

    public void EnterPlayState()
    {
        robotScript.currentState = robotScript.playState;
        if (robotScript.commands.Count > 0)
        {
            Debug.Log("entering robotplaystate");
            robotScript.DecideCommand();
        }   
    }
    public void UpdateState()
    {
        //dont execute since we are in pausestate
    }
}
