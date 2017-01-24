using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotBehaviour : MonoBehaviour {

    public IRobotState currentState;
    public PauseState pauseState;
    public PlayState playState;
    public delegate void ClickedOnRobot(GameObject robot);
    public static event ClickedOnRobot OnClick;
    //the robot goes through each commando and checks each update if the latest commando is finished or not
    //if it is finished then the robot starts the next commando
    public List<Command> commands;
    //when switching commands, call the FinishedCoroutine coroutine
    //robot's playstate's updatestate calls the current command's execute
    private Command currentCommand;
    void Awake()
    {
        commands = new List<Command>();
        pauseState = new PauseState(gameObject);
        playState = new PlayState(gameObject);
        currentState = pauseState;
    }
    void Start()
    {
       
    }
    void FixedUpdate()
    {
        currentState.UpdateState();
    }
    /// <summary>
    /// Picks the oldest command if possible and 
    /// starts the commands lifetimetimer
    /// </summary>
    public void DecideCommand()
    {
        Debug.Log(commands.Count);
        if(commands.Count > 0)
        {
            if (commands[0].isFinished == false)
            {
                currentCommand = commands[0];
                //begin the lifetime timer on currentCommand
                StartCoroutine(currentCommand.FinishedCoroutine());
            }
            
        }
    }
    public void ExecuteRobotCommand()
    {
        if (currentCommand != null && currentCommand.isFinished)
        {
            //remove the currentcommand from the command list
            commands.Remove(currentCommand);
            currentCommand = null;
            DecideCommand();
        }
        else if(currentCommand != null)
        {
            currentCommand.Execute();
        }
    }
    void OnMouseDown()
    {
        OnClick(gameObject);
    }
}

