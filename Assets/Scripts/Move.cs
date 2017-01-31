using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Move {
    //ett drag

    public GameObject robot;
    public Vector2 position;
    public Vector2 velocity;
    public List<Command> commands;
    public int turn;
    public Quaternion rotation;
    public float angularVelocity;
    public Move(GameObject robot, int turn, List<Command> commands)
    {
        this.robot = robot;
        this.turn = turn;
        this.position = robot.transform.position;
        this.velocity = robot.GetComponent<Rigidbody2D>().velocity;
        this.commands = new List<Command>();
        this.commands.AddRange(commands);
        foreach(Command c in this.commands)
        {
            c.isFinished = false;
            c.lifeTimer = c.lifeDuration;
        }
        this.rotation = robot.transform.rotation;
        this.angularVelocity = robot.GetComponent<Rigidbody2D>().angularVelocity;
    }
}
