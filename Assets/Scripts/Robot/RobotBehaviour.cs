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

    private Animator animatorComponent;
    private Rigidbody2D rigidBodyComponent;

    public Vector2 prevVelocity;

    string idle = "Idle";
    string[] movingDirectionAnimations;


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

    public void UpdateAnimationAndCollider()
    {
        float maximumAngle = Mathf.PI * 2;
        int nAnimations = movingDirectionAnimations.Length;
        float interval = maximumAngle / nAnimations;
        float directionAngle = Mathf.Atan2(rigidBodyComponent.velocity.y, rigidBodyComponent.velocity.x);
        //Vector2 direction = rigidBodyComponent.velocity.normalized;

        if (directionAngle < 0)
        {
            directionAngle += maximumAngle;
        }

        float smallestAngle = 0, biggestAngle = interval;
        for (int i = 0; i < nAnimations; i++)
        {
            animatorComponent.SetBool(movingDirectionAnimations[i], false);
            if (directionAngle > smallestAngle && directionAngle < biggestAngle)
            {
                animatorComponent.SetBool(movingDirectionAnimations[i], true);
                for (int j = i + 1; j < nAnimations; j++)
                {
                    animatorComponent.SetBool(movingDirectionAnimations[j], false);
                }
                return;
            }
            smallestAngle += interval;
            biggestAngle += interval;
        }
    }

    void Awake()
    {
        if(GetComponent<Rigidbody2D>() == null)
        {
            gameObject.AddComponent<Rigidbody2D>();
        }
        rigidBodyComponent = GetComponent<Rigidbody2D>();
        movingDirectionAnimations = new string[] { "Move Right", "Move Right Upper Diagonal", "Move Up" , "Move Left Upper Diagonal" , "Move Left" , "Move Left Lower Diagonal", "Move Down", "Move Right Lower Diagonal" };
        animatorComponent = GetComponent<Animator>();
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
}

