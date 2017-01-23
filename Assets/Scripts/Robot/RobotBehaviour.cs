using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotBehaviour : MonoBehaviour {

    public IRobotState currentState;
    public PauseState pauseState;
    public PlayState playState;

    //the robot goes through each commando and checks each update if the latest commando is finished or not
    //if it is finished then the robot starts the next commando
    public List<Commando> commands;
    void Start()
    {
        pauseState = new PauseState(gameObject);
        playState = new PlayState(gameObject);
        currentState = pauseState;
        commands = new List<Commando>();
       
    }
    void Update()
    {
        if(commands[0] != null && commands[0].isFinished == false)
        {
            Debug.Log("executing");
            commands[0].Execute();
        }
        else
        {
            Debug.Log("command 0 was null or finished");
        }
        currentState.updateState();
    }
  
}

