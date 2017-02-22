using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Move {

	private int turn;
    public Vector2 position;
    public Quaternion rotation;
	public List<Command> commands;

	private Vector2 velocity;
	public Vector2 Velocity { get { return velocity; } }
    private float angularVelocity;
    public float AngularVelocity { get { return angularVelocity; } }
	private GameObject robot;
	public GameObject Robot { get { return robot; } }

    public Move (GameObject robot, int turn, List<Command> commands)
	{
        this.robot = robot;
        this.turn = turn;
        this.position = robot.transform.position;
        this.velocity = robot.GetComponent<Rigidbody2D>().velocity;
        this.commands = new List<Command>();
        this.commands.AddRange(commands);
        foreach(Command c in this.commands){
            c.isFinished = false;
            c.lifeTimer = c.lifeDuration;
        }
        this.rotation = robot.transform.rotation;
        this.angularVelocity = robot.GetComponent<Rigidbody2D>().angularVelocity;
    }
}