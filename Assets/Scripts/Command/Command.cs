using UnityEngine;
using System.Collections;

public abstract class Command {

    public GameObject robot;
    //execute is called by the robot
    public abstract void Execute();
    public bool isFinished;
    public abstract IEnumerator FinishedCoroutine();
    public Vector2 targetPosition;
}
