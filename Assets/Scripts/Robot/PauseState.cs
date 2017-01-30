using UnityEngine;
using System.Collections;
using System;

public class PauseState : IRobotState
{
    private GameObject robot;
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

        robotScript.CurrentState = robotScript.playState;
   
        robotScript.DecideCommand();
          
    }
    public void UpdateState()
    {

    }
}
