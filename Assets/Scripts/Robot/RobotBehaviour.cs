using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RobotBehaviour : MonoBehaviour {

    private IRobotState currentState;
    public PauseState pauseState;
    public PlayState playState;
    public delegate void ClickedOnRobot(GameObject robot);
    public static event ClickedOnRobot OnClick;
    [HideInInspector]
    public bool shouldSendEvent = false;
    //the remaining time for this round
    [HideInInspector] //it is set by turnhandler's roundTime
    public float freeTime;
    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public Vector2 prevVelocity;
    [HideInInspector]
    public Animator anim;
    public float angleAtPause;

    //the robot goes through each commando and checks each update if the latest commando is finished or not
    //if it is finished then the robot starts the next commando
    List<Command> commands;
    public List<Command> oldCommands;
    //when switching commands, call the FinishedCoroutine coroutine
    //robot's playstate's updatestate calls the current command's execute
    private Command currentCommand;

    private Animator animatorComponent;

    private Vector2 startPosition;
    /// <summary>
    /// the acceleration used by movecommand to move the robot
    /// </summary>
    public float moveCommandAcceleration;

    public bool isPreview;
	

    //AUDIOSOURCES
    [SerializeField]
    GameObject collideRobotSound;
    [SerializeField]
    public GameObject thrusterSound;
    [SerializeField]
    public GameObject igniteThrustersSound;
    [SerializeField]
    public GameObject endThrustersSound;
    [SerializeField]
    GameObject selectRobotSound;
    [HideInInspector]
    public AudioSource thrusterComponent;

    [HideInInspector]
    //helps with onaccelerate and ondeaccelerate
    public bool accelerated = false;


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

    public void UpdateAnimationAngle(float y, float x)
    {
        if (animatorComponent.runtimeAnimatorController != null)
        {
            
            if(y <= 0.2f && y >= -0.2f && x <= 0.2f && x >= -0.2f){
                //if we are not moving, set it to downwards
                animatorComponent.SetFloat("DirectionAngle", 256f);
                animatorComponent.SetBool("Still", true);
                
            }
            else{
                animatorComponent.SetBool("Still", false);
                float directionAngle = Mathf.Atan2(y, x);

                directionAngle = directionAngle * Mathf.Rad2Deg;
                if (directionAngle < 0.0f)
                {
                    directionAngle += 360.0f;
                }

                string animationVariable = "DirectionAngle";
                if(directionAngle >= 345 || directionAngle <= 15)
                {
                    directionAngle = 359;
                }
               
                animatorComponent.SetFloat(animationVariable, directionAngle);
                }
            }
    }

    void Awake()
    {
		rb = GetComponent<Rigidbody2D>();
		prevVelocity = rb.velocity;
		animatorComponent = GetComponent<Animator>();
        Commands = new List<Command>();
        oldCommands = new List<Command>();
        pauseState = new PauseState(gameObject);
        playState = new PlayState(gameObject);
        currentState = pauseState;
        commands = new List<Command>();

        anim = GetComponent<Animator>();
        
        thrusterComponent = GetComponent<AudioSource>();


        // reset position when a goal is made
        if(isPreview == false)
        {
            Goal.OnGoalScored += new Goal.GoalScored(ResetRobotAfterScore);
        }
        
    }
    void ResetRobotAfterScore()
    {
        anim.SetBool("Accelerating", false);
        //anim.Play("Idle", 0);
        transform.position = startPosition;
		rb.velocity = Vector2.zero;
		prevVelocity = Vector2.zero;
        commands.Clear();
        currentCommand = null;
        
    }
    void Start()
    {
        startPosition = transform.position;

        //UpdateAnimationAngle(0f,0f); 
        anim.enabled = false;

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
        if (Commands != null && Commands.Count > 0)
        {
            if (Commands[0] != null && Commands[0].isFinished == false)
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
			Commands.Remove(currentCommand); //remove currentCommand from the command list
            currentCommand = null;
            DecideCommand();
        }
        else if(currentCommand != null)
        { 
            currentCommand.Execute();
           
        }
    }
    public void OnAccelerate()
    {
        currentState.OnAccelerate(); 
    }
    public void OnDeaccelerate()
    {
        currentState.OnDeaccelerate();
    }
    void OnMouseDown()
    {
        if (OnClick != null && shouldSendEvent)
        {
            OnClick(gameObject);
            AudioManager.instance.PlayAudioWithRandomPitch(selectRobotSound, false, gameObject);
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if(isPreview == false)
        {
            UpdateAnimationAngle(rb.velocity.y,rb.velocity.x);
            if (other.gameObject.tag == "Robot")
            {
                AudioManager.instance.PlayAudioWithRandomPitch(collideRobotSound, false, gameObject);
            }
        }
    }
    void OnDestroy()
    {
        
        Goal.OnGoalScored -=  ResetRobotAfterScore;
    }
}

