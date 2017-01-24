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

    public Move(GameObject robot, int turn, List<Command> commands)
    {
        this.robot = robot;
        this.turn = turn;
        this.position = robot.transform.position;
        this.velocity = robot.GetComponent<Rigidbody2D>().velocity;
        this.commands = commands;
    }
}
