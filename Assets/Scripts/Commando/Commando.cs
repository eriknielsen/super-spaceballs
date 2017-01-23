using UnityEngine;
using System.Collections;

public abstract class Commando {

    public GameObject robot;
    //execute is called by the robot
    public abstract void Execute();
    public bool isFinished;

}
