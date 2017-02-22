﻿using UnityEngine;
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

    //AUDIOSOURCES
    [SerializeField]
    GameObject collideRobotSound;
    [SerializeField]
    GameObject moveSound;


    float speed;

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
        
    }
    void Start()
    {
       
    }
    void FixedUpdate()
    {
        
        //anim.SetFloat("Speed", Math.Abs(rb.velocity.x + rb.velocity.y));
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
            if (Commands != null && Commands[0].isFinished == false)
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
            if (currentCommand.GetType() == typeof(MoveCommand))
            {
                anim.SetBool("Accelerating", true);
            }
            else
            {
                anim.SetBool("Accelerating", false);
            }
        }
        else
        {
            anim.SetBool("Accelerating", false);
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
        if (other.tag == "Robot")
        {
            //play a soud here!
            AudioManager.instance.PlayAudioWithRandomPitch(collideRobotSound, false, gameObject);
            anim.SetTrigger("Push");
            anim.ResetTrigger("Push");
        }
    }
}

