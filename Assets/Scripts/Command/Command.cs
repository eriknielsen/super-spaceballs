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
}
