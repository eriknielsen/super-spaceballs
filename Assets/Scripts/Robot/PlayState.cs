using UnityEngine;
using System.Collections;
using System;

public class PlayState : IRobotState {
    public GameObject robot;
    private RobotBehaviour robotScript;
    public PlayState(GameObject r)
    {
        robot = r;
        robotScript = robot.GetComponent<RobotBehaviour>();
    }
    public void EnterPauseState()
    {
        robotScript.currentState = robotScript.pauseState;
        //empty the command list?
        robotScript.commands.Clear();
    }
    public void EnterPlayState()
    {
        Debug.Log("already in playstate");
       
    }
    public void UpdateState()
    {
        //Debug.Log(robot.GetComponent<Rigidbody2D>().velocity);
        
        if (robotScript.currentCommand.isFinished)
        {
            robotScript.DecideCommand();
        }
        //if currentcommand is null there are no more commands to execute
        if (robotScript.currentCommand != null)
            robotScript.currentCommand.Execute();
    }
}
