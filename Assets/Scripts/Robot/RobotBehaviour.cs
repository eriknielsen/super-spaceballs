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
    public bool shouldSendEvent = false;
    //the remaining time for this round
    public float freeTime;

    public Vector2 prevVelocity;
    public Animator anim;
    //the robot goes through each commando and checks each update if the latest commando is finished or not
    //if it is finished then the robot starts the next commando
    public List<Command> commands;
    public List<Command> oldCommands;
    //when switching commands, call the FinishedCoroutine coroutine
    //robot's playstate's updatestate calls the current command's execute
    private Command currentCommand;

    private Animator animatorComponent;
    private Rigidbody2D rigidBodyComponent;

    private Vector2 startPosition;

    public bool isPreview;
	private float speed;
	Rigidbody2D rb;

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
        if (animatorComponent.runtimeAnimatorController != null)
        {
            float directionAngle = Mathf.Atan2(rigidBodyComponent.velocity.y, rigidBodyComponent.velocity.x);

            directionAngle = directionAngle * Mathf.Rad2Deg;
            if (directionAngle < 0.0f)
            {
                directionAngle += 360.0f;
            }
            string animationVariable = "DirectionAngle";
            animatorComponent.SetFloat(animationVariable, directionAngle);
        }
    }

    void Awake()
    {
        
        if(GetComponent<Rigidbody2D>() == null)
        {
            gameObject.AddComponent<Rigidbody2D>();
        }
        prevVelocity = gameObject.GetComponent<Rigidbody2D>().velocity;
        rigidBodyComponent = GetComponent<Rigidbody2D>();
        animatorComponent = GetComponent<Animator>();
        Commands = new List<Command>();
        oldCommands = new List<Command>();
        pauseState = new PauseState(gameObject);
        playState = new PlayState(gameObject);
        currentState = pauseState;
        rb = GetComponent<Rigidbody2D>();
        commands = new List<Command>();

        anim = GetComponent<Animator>();
        anim.enabled = false;

        thrusterComponent = GetComponent<AudioSource>();
        // reset position when a goal is made
        //ONLY IF WE ARE NOT A PREVIEW. PREVIEWS DONT HAVE THAT COMPONENT
        if(isPreview == false)
        {
            Goal.OnGoalScored += new Goal.GoalScored(() => transform.position = startPosition);
        }
        
    }
    void Start()
    {
        startPosition = transform.position;
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
            if (currentCommand.GetType() == typeof(MoveCommand))
            {
                OnAccelerate();
            }
            else
            {
                OnDeaccelerate();
            }
        }
        else
        {
            anim.SetBool("Accelerating", false);
        }
    }
    void OnAccelerate()
    {
        currentState.OnAccelerate(); 
    }
    void OnDeaccelerate()
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
            if (other.gameObject.tag == "Robot")
            {
                AudioManager.instance.PlayAudioWithRandomPitch(collideRobotSound, false, gameObject);
            }
        }
        
        
    }
}

