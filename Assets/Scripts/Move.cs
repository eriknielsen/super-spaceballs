using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Move {
    //ett drag

    private GameObject robot;
    public Vector2 position;
    private Vector2 velocity;
    public List<Command> commands;
    private int turn;
    public Quaternion rotation;
    private float angularVelocity;
    
    public float AngularVelocity
    {
        get { return angularVelocity; }
    }

    public GameObject Robot
    {
        get { return robot;  }
    }

    public Vector2 Velocity
    {
        get { return velocity; }
    }

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
