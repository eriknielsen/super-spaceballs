using UnityEngine;
using System.Collections;

public class TurnHandlerBehaviour : MonoBehaviour {

    public GameObject testRobot;
    public RobotBehaviour robot;

	// Use this for initialization
	void Start () {
        TestRobotCommands();

    }
	void TestRobotCommands()
    {
        if(testRobot != null)
        {
            Debug.Log("testing the move command");
            Command moveForward = new MoveCommand(testRobot, new Vector2(3, 3),1);
            Command moveBack = new MoveCommand(testRobot, new Vector2(-1, 0), 1);
            robot = testRobot.GetComponent<RobotBehaviour>();
            robot.commands.Add(moveForward);
            robot.commands.Add(moveBack);
            robot.currentState.EnterPlayState();
        }
    }
	// Update is called once per frame
	void Update () {
	
	}
}
