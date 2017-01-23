using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotBehaviour : MonoBehaviour {

    public IRobotState currentState;
    public PauseState pauseState;
    public PlayState playState;

    //the robot goes through each commando and checks each update if the latest commando is finished or not
    //if it is finished then the robot starts the next commando
    public List<Command> commands;
    //when switching commands, call the FinishedCoroutine coroutine
    //robot's playstate's updatestate calls the current command's execute
    public Command currentCommand = null;
    void Start()
    {
        pauseState = new PauseState(gameObject);
        playState = new PlayState(gameObject);
        currentState = pauseState;
        commands = new List<Command>();
    }
    void FixedUpdate()
    {
        currentState.UpdateState();
    }
    public void DecideCommand()
    {
        bool search = true;
        Debug.Log(commands.Count);
        foreach (Command c in commands)
        {
            if (search == true && c.isFinished == false)
            {
                search = false;
                currentCommand = c;
                StartCoroutine(currentCommand.FinishedCoroutine());
                
            }
        }
    }
}

