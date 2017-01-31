﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnHandlerBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject robotPrefab;
    [SerializeField]
    GameObject shockWavePrefab;
    [SerializeField]
    private int numberOfRobots;
    [SerializeField]
    float roundTime;

    private GameObject selectedRobot;
    private AvailableCommands selectedCommand;
    private enum AvailableCommands { MoveCommand, PushCommand };

    List<Entity> entities;
    private List<GameObject> robots;
    private int turns;

    float robotWidth;
    float robotHeight;

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
        robotHeight = robotPrefab.GetComponent<SpriteRenderer>().bounds.max.y;
        robotWidth = robotPrefab.GetComponent<SpriteRenderer>().bounds.max.x;
        bc2D = GetComponent<BoxCollider2D>();
        selectedCommand = AvailableCommands.PushCommand;
        moves = new List<Move>();
        robots = new List<GameObject>();
        entities = new List<Entity>();

        CreateRobots();
        turns = 1;

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


        //remove IEntities who's gameobjects have been removed
        //and pause the rest
        List<Entity> newList = new List<Entity>();
        for(int i = 0; i < entities.Count; i++)
        {
            if (entities[i] != null)
                newList.Add(entities[i]);

        }
        entities = newList;
        for(int i = 0;i < entities.Count; i++)
        {
           
            if(entities[i] != null)
            {
                entities[i].EnterPause();
            }
            else
            {
                
            }
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
            //save the robots position,velocity, commands etc
            moves.Add(new Move(r, Turns, r.GetComponent<RobotBehaviour>().Commands));
            
            r.GetComponent<RobotBehaviour>().CurrentState.EnterPlayState();
            
        }
        //also unpause all entities
        foreach (Entity e in entities)
        {
            if(e != null)
            {
                e.EnterPlay();
            }
            else {
                Debug.Log("entity was null");
            }
                
        }
        turns++;
    }


    void UndoLastMove()
    {
        if (Turns > 0)
        {
            
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
                Debug.Log("MoveCommand Added!");
            }
            if(selectedCommand == AvailableCommands.PushCommand)
            {
                Vector3 mousePosition = Input.mousePosition;
                Vector3 pointPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                selectedRobot.GetComponent<RobotBehaviour>().Commands.Add(new PushCommand(selectedRobot, pointPosition, 1, Turns));
                Debug.Log("PushCommand Added!");
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
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("movecommand chosen");
            selectedCommand = AvailableCommands.MoveCommand;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("pushcommand chosen");
            selectedCommand = AvailableCommands.PushCommand;
        }
    }
    public void Activate(bool activate)
    {
        if (activate == true)
        {
            //visually indicate that this turnhandlers robots are now active
       
            //start taking events
            RobotBehaviour.OnClick += new RobotBehaviour.ClickedOnRobot(ChooseRobot);
            PushCommand.OnInstantiateShockWave += new PushCommand.InstantiateShockWave(InstantiateShockwave);

            foreach (GameObject r in robots)
            {
                r.GetComponent<RobotBehaviour>().shouldSendEvent = true;
            }
            enabled = true;
        }
        else
        {
           
            RobotBehaviour.OnClick -= new RobotBehaviour.ClickedOnRobot(ChooseRobot);
            foreach (GameObject r in robots)
            {
                r.GetComponent<RobotBehaviour>().shouldSendEvent = false;
            }
            enabled = false;
        }

    }
    public void ReplayLastTurn()
    {
        //save commando lists in robots where they are longer than 0
        //and put them in that robots oldCommands<List>
        foreach(GameObject r in robots)
        {
            if(r.GetComponent<RobotBehaviour>().Commands.Count > 0)
            {
                r.GetComponent<RobotBehaviour>().oldCommands = r.
                    GetComponent<RobotBehaviour>().Commands;
            }
        }
        //go through the last 8 moves and move each robot to their old position
        for(int i = moves.Count-1; i > moves.Count - numberOfRobots-1; i--)
        {

            Move m = moves[i];
            GameObject r = m.robot;
            r.GetComponent<Rigidbody2D>().angularVelocity = m.angularVelocity;
            r.GetComponent<Rigidbody2D>().velocity = m.velocity;
            r.transform.position = m.position;
            r.transform.rotation = m.rotation;
           
            r.GetComponent<RobotBehaviour>().Commands.AddRange(m.commands);
        }

        
    }
    public void RevertToOldCommands()
    {
        foreach (GameObject r in robots)
        {
            if (r.GetComponent<RobotBehaviour>().oldCommands.Count > 0)
            {
                RobotBehaviour robot = r.GetComponent<RobotBehaviour>();
                robot.commands = robot.oldCommands;
                robot.oldCommands.Clear();

            }
        }
    }
    void InstantiateShockwave(GameObject robot, Vector2 dir, float chargeTime)
    {
        if (robots.IndexOf(robot) != -1)
        {
            if (shockWavePrefab != null)
            {
                
                Vector3 offsettedPosition = new Vector3(dir.normalized.x * robotWidth/3, dir.normalized.y * robotHeight/3) + robot.transform.position;

                GameObject sw = Instantiate(shockWavePrefab, offsettedPosition, new Quaternion()) as GameObject;
                
                ShockwaveBehaviour svbh = sw.GetComponent<ShockwaveBehaviour>();
                entities.Add(svbh);
                svbh.extraChargeForce = chargeTime * 2;
                
                svbh.direction = dir.normalized;
                //change rotation

            }
            else
            {
                Debug.Log("shockwaveprefab was null");
            }
        }
        else
        {
            //not intended for this instance
        }
       

    }
}
