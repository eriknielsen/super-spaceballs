﻿using UnityEngine;
using System.Collections;
using System;

public class PauseState : IRobotState {
	
    private GameObject robot;
    private RobotBehaviour robotScript;
   

    public PauseState(GameObject r){
        robot = r;
        robotScript = robot.GetComponent<RobotBehaviour>();
    }

    public void EnterPauseState(){
        Debug.Log("already in pause");
    }

    public void EnterPlayState(){

        robotScript.CurrentState = robotScript.playState;
        robot.GetComponent<RobotBehaviour>().anim.enabled = true;
        robotScript.DecideCommand();
        robot.GetComponent<Rigidbody2D>().velocity = robotScript.prevVelocity;
    }

    public void UpdateState(){

    }
    public void OnAccelerate()
    {

    }
    public void OnDeaccelerate()
    {

    }
}
