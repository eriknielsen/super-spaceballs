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
    }

    public void EnterPlayState()
    {
        throw new NotImplementedException();
    }

    public void UpdateState()
    {
        throw new NotImplementedException();
    }

 
}
