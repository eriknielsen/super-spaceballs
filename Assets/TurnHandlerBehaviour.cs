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
            Commando move = new MoveCommando(testRobot, new Vector2(3, 3));
            robot = testRobot.GetComponent<RobotBehaviour>();
            robot.commands.Add(move);
        }
    }
	// Update is called once per frame
	void Update () {
	
	}
}
