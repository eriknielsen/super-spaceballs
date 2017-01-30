using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotBehaviour : MonoBehaviour {

    private IRobotState currentState;
    public PauseState pauseState;
    public PlayState playState;
    public delegate void ClickedOnRobot(GameObject robot);
    public static event ClickedOnRobot OnClick;
    public bool shouldSendEvent = false;
    //the robot goes through each commando and checks each update if the latest commando is finished or not
    //if it is finished then the robot starts the next commando
    public List<Command> commands;
    public List<Command> oldCommands;
    //when switching commands, call the FinishedCoroutine coroutine
    //robot's playstate's updatestate calls the current command's execute
    private Command currentCommand;

    Rigidbody2D rb;
    public IRobotState CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    public List<Command> Commands
    {
        get { return commands; }
        set { commands = value; }
    }

    void Awake()
    {
        Commands = new List<Command>();
        oldCommands = new List<Command>();
        pauseState = new PauseState(gameObject);
        playState = new PlayState(gameObject);
        currentState = pauseState;
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
       
    }
    void FixedUpdate()
    {
        CurrentState.UpdateState();
    }
    /// <summary>
    /// Picks the oldest command if possible and 
    /// starts the commands lifetimetimer
    /// </summary>
    public void DecideCommand()
    {
       
        if (Commands.Count > 0)
        {
            
            if (Commands[0].isFinished == false)
            {
               
                currentCommand = Commands[0];
                //begin the lifetime timer on currentCommand
               
            }
            
        }
    }
    
    public void ClearCommands()
    {
        Commands.Clear();
    }

    public void ExecuteRobotCommand()
    {
       
        if (currentCommand != null && currentCommand.isFinished)
        {
            //remove the currentcommand from the command list
            Commands.Remove(currentCommand);
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
        if (OnClick != null && shouldSendEvent)
        {
            OnClick(gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Shockwave")
        {
            //apply some force
            rb.AddForce(other.gameObject.GetComponent<ShockwaveBehaviour>().pushForce
                * other.gameObject.GetComponent<Rigidbody2D>().velocity.normalized);
            Debug.Log("hit by shockwave!");
        }
    }
}

