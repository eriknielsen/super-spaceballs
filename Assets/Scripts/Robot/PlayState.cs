using UnityEngine;
using System.Collections;
using System;

public class PlayState : IRobotState {
    private GameObject robot;
    private RobotBehaviour robotScript;

    Vector2 zeroVector;
    public PlayState(GameObject r)
    {
        zeroVector = new Vector2(0, 0);
        robot = r;
        robotScript = robot.GetComponent<RobotBehaviour>();
    }
    public void EnterPauseState()
    {
        robotScript.CurrentState = robotScript.pauseState;
        //empty the command list
        robotScript.Commands.Clear();
        robotScript.prevVelocity = robot.GetComponent<Rigidbody2D>().velocity;
        robot.GetComponent<Rigidbody2D>().velocity = zeroVector;
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
