﻿using UnityEngine;
using System.Collections;
using System;

public class PlayState : IRobotState {
    private GameObject robot;
    private RobotBehaviour robotScript;
    public PlayState(GameObject r)
    {
        robot = r;
        robotScript = robot.GetComponent<RobotBehaviour>();
    }
    public void EnterPauseState()
    {
        robotScript.currentState = robotScript.pauseState;
        //empty the command list
        robotScript.commands.Clear();
    }
    public void EnterPlayState()
    {
        Debug.Log("already in playstate");
       
    }
    public void UpdateState()
    {
        robotScript.ExecuteRobotCommand();
    }
}
