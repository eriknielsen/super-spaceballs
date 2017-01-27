﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnHandlerBehaviour : MonoBehaviour
{
    [SerializeField]
    private RobotBehaviour robotPrefab;
    [SerializeField]
    private int numberOfRobots;
    [SerializeField]
    float roundTime;

    private GameObject selectedRobot;
    private AvailableCommands selectedCommand;
    private enum AvailableCommands { MoveCommand };

    private List<GameObject> robots;
    private int turns;

    BoxCollider2D bc2D;

    //en lista med drag
    public List<Move> moves;
    

    public int Turns
    {
        get
        {
            return turns;
        }
    }

    void OnValidate()
    {
        if (roundTime < 0)
        {
            roundTime = 0;
        }
    }

    void Awake()
    {
        bc2D = GetComponent<BoxCollider2D>();
        selectedCommand = AvailableCommands.MoveCommand;
        moves = new List<Move>();
        robots = new List<GameObject>();


        CreateRobots();
        turns = 1;
        

        
        
    }
    void Start()
    {
       
    }
    void CreateRobots()
    {
        if (robotPrefab != null)
        {
            for (int i = 0; i < numberOfRobots; i++)
            {
                float x = Random.Range(bc2D.bounds.min.x, bc2D.bounds.max.x);
                float y = Random.Range(bc2D.bounds.min.y, bc2D.bounds.max.y);
                GameObject r = Instantiate(robotPrefab.gameObject, new Vector2(x, y), new Quaternion()) as GameObject;
                if (robots == null)
                {
                    Debug.Log("Null as fuuuuck");
                }
                robots.Add(r);
            }

        }
        bc2D.enabled = false;
    }

    public void PauseGame()
    {
        //put all robots into pausestate
        foreach (GameObject r in robots)
        {
            r.GetComponent<RobotBehaviour>().CurrentState.EnterPauseState();
        }
    }
    public void UnpauseGame()
    {
        //put all robots into play
        int i = 0;
        foreach (GameObject r in robots)
        {
            if (r == null)
            {
                Debug.Log("Null at " + i);
            }
            i++;
            moves.Add(new Move(r, Turns, r.GetComponent<RobotBehaviour>().Commands));
            r.GetComponent<RobotBehaviour>().CurrentState.EnterPlayState();
        }
        turns++;
    }


    void UndoLastMove()
    {
        if (Turns > 0)
        {
            //remove all shockwaves
            //reset all robots to the previous' move's position
            int turnIndex = 0;
            for (int i = Turns - 1; i < Turns * robots.Count; i++)
            {

                turnIndex = (Turns - 1) * robots.Count + i;
                robots[i].transform.position = moves[turnIndex].position;
                robots[i].GetComponent<Rigidbody2D>().velocity = moves[turnIndex].velocity;
            }
        }

    }

    void ChooseRobot(GameObject r)
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectedRobot = r;
            Debug.Log("Robot selected!");
        }
    }

    void GiveCommandToSelectedRobot()
    {
        if (selectedRobot != null)
        {
            if (selectedCommand == AvailableCommands.MoveCommand)
            {
                Vector3 mousePosition = Input.mousePosition;
                Vector3 pointPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                selectedRobot.GetComponent<RobotBehaviour>().Commands.Add(new MoveCommand(selectedRobot, pointPosition, 2, Turns));
                Debug.Log("Command Added!");
            }
        }
    }
    void Update()
    {
        ReactToUserInput();
    }

    void ReactToUserInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GiveCommandToSelectedRobot();
        }
    }
    public void Activate(bool activate)
    {
        if (activate == true)
        {
            //visually indicate that this turnhandlers robots are now active
            Debug.Log("select robot");
            //start taking events
            RobotBehaviour.OnClick += new RobotBehaviour.ClickedOnRobot(ChooseRobot);

            foreach (GameObject r in robots)
            {
                r.GetComponent<RobotBehaviour>().shouldSendEvent = true;
            }
        }
        else
        {
            Debug.Log("stop selecting robot");
            RobotBehaviour.OnClick -= new RobotBehaviour.ClickedOnRobot(ChooseRobot);
            foreach (GameObject r in robots)
            {
                r.GetComponent<RobotBehaviour>().shouldSendEvent = false;
            }
        }

    }
}
