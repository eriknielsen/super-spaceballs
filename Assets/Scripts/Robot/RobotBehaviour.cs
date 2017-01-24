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
    public Command currentCommand = null;
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
    public void DecideCommand()
    {
        bool search = true;

        foreach (Command c in commands)
        {
            if (search == true && c.isFinished == false)
            {
                search = false;
                currentCommand = c;
                StartCoroutine(currentCommand.FinishedCoroutine());
                
            }
            
        }
        if(search == true)
        {
            //no commands where found
            currentCommand = null;
        }

    }
    void OnMouseDown()
    {
        OnClick(gameObject);
      
    }
}

