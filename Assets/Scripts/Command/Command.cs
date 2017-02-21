using UnityEngine;
using System.Collections;

public abstract class Command {

    public GameObject robot;
    //execute is called by the robot
    public abstract void Execute();
    public bool isFinished = false;
    public float lifeDuration;
    public float lifeTimer;
  
    public Vector2 targetPosition;
    public int turn;

    public GameObject Robot
    {
        get { return robot; }
    }
}

[System.Serializable]
public class SerializableCommand {

    public enum CommandType { Move,Push};
    public CommandType type;
    public int robotIndex;
    public Position targetPosition;
    public float lifeDuration;
    public int turn;
    public SerializableCommand(int index, Vector2 targetPos, float duration, CommandType t, int turn)
    {
        robotIndex = index;
        targetPosition = new Position(targetPos);
        lifeDuration = duration;
        this.turn = turn;
        type = t;
    }
}
