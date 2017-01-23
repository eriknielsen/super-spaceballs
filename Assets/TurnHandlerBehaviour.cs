using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnHandlerBehaviour : MonoBehaviour {

    public GameObject testRobot;
    public RobotBehaviour robot;
    public List<GameObject> robots;
    public int turns;

	// Use this for initialization
	void Start () {
        //TestRobotCommands();
        CreateTestRobots();
        //remember which commands were for which turn
        turns = 1;

    }
	void TestRobotCommands()
    {
        if(testRobot != null)
        {
            Debug.Log("testing the move command");
           List<Command> testCommands = new List<Command>();
          
            foreach (GameObject r in robots)
            {
                Command moveForward = new MoveCommand(r, new Vector2(3, 3), 1,turns);
                Command moveBack = new MoveCommand(r, new Vector2(-1, 0), 1,turns);

                r.GetComponent<RobotBehaviour>().commands.Add(moveForward);
                r.GetComponent<RobotBehaviour>().commands.Add(moveBack);
            } 
        }
    }
    void CreateTestRobots()
    {
        for(int i = 0;i < 3; i++)
        {
            Instantiate(testRobot, new Vector2(i, i+1), new Quaternion());
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
            r.GetComponent<RobotBehaviour>().currentState.EnterPlayState();
        }
        turns++;
    }
}
