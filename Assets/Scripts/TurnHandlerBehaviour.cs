﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnHandlerBehaviour : MonoBehaviour {

    public GameObject testRobot;
    public List<GameObject> robots;
    public int turns;
    //en lista med drag
    public List<Move> moves;
	// Use this for initialization
	void Start () {
        moves = new List<Move>();
        RobotBehaviour.OnClick += new RobotBehaviour.ClickedOnRobot(ChooseRobot);
        CreateTestRobots();
        TestRobotCommands();
        UnpauseGame();
        //remember which commands were for which turn
        turns = 1;
        StartCoroutine(TestPausing());

    }
	void TestRobotCommands()
    {
        
        
        List<Command> testCommands = new List<Command>();
          
        foreach (GameObject r in robots)
        {
            Debug.Log("testing the move command");
            Command moveForward = new MoveCommand(r, new Vector2(3, 3), 1,turns);
            Command moveBack = new MoveCommand(r, new Vector2(-1, 0), 1,turns);

            r.GetComponent<RobotBehaviour>().commands.Add(moveForward);
            r.GetComponent<RobotBehaviour>().commands.Add(moveBack);
        } 
        
    }
    void CreateTestRobots()
    {
        for(int i = 0;i < 3; i++)
        {
            GameObject r = Instantiate(testRobot, new Vector2(i+1, i+1), new Quaternion()) as GameObject;
            robots.Add(r);
        }
    }
	// Update is called once per frame
	void Update () {
	    
	}
    public void PauseGame()
    {
        //change timescale
        Time.timeScale = 0;
        //put all robots into pausestate
        foreach(GameObject r in robots)
        {
           
            r.GetComponent<RobotBehaviour>().currentState.EnterPauseState();
        }
    }
    public void UnpauseGame()
    {
        //change timescale
        Time.timeScale = 1;
        //put all robots into play
        foreach (GameObject r in robots)
        {
            moves.Add(new Move(r, turns, r.GetComponent<RobotBehaviour>().commands));
            r.GetComponent<RobotBehaviour>().currentState.EnterPlayState();
        }
        turns++;
    }

    IEnumerator TestPausing()
    {
        yield return new WaitForSeconds(4f);
        //PauseGame();
       
        //UndoLastMove();
        
    }
    void UndoLastMove()
    {
        //remove all shockwaves
        //reset all robots to the previous' move's position
        for(int i = 1; i < robots.Count; i++)
        {
            Debug.Log("hej");
            robots[i].transform.position = moves[i-1].position;
            robots[i].GetComponent<Rigidbody2D>().velocity = moves[i-1].velocity;
        }
    }
    void ChooseRobot(GameObject r)
    {
        Debug.Log(r);
    }
}
