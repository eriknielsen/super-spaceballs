﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class TurnHandlerBehaviour : MonoBehaviour
{

    [SerializeField]
    GameObject shockWavePrefab;
    [SerializeField]
    private int numberOfRobots;
    [HideInInspector]
    public float roundTime;

    private Text cursorText;

    private GameObject selectedRobot;
    private int selectedRobotIndex;
    private AvailableCommands selectedCommand;
    private enum AvailableCommands { MoveCommand, PushCommand };

    List<GameObject> robots;
    int turns;
    bool mouseButtonIsPressed = false;
    BoxCollider2D bc2D;
    float timeInput;
    List<GameObject> robotsMovingPreviews;
    List<List<GameObject>> robotsPreview;
    MovingTrail latestTrail;

    bool activated = false;
    //en lista med drag
    public List<Move> moves;

    public int Turns
    {
        get
        {
            return turns;
        }
    }
    public List<GameObject> Robots
    {
        get { return robots; }

    }
    public int NumberOfRobots
    {
        get
        {
            return numberOfRobots;
        }
    }

    void Awake()
    {
        selectedCommand = AvailableCommands.MoveCommand;
        moves = new List<Move>();
        robots = new List<GameObject>();
        CreateRobots(numberOfRobots);
        robotsMovingPreviews = new List<GameObject>();
        robotsPreview = new List<List<GameObject>>();
        for (int i = 0; i < robots.Count; i++)
        {
            robotsMovingPreviews.Add(new GameObject());
            robotsMovingPreviews[i].name = "Robot moving trail";
            robotsMovingPreviews[i].SetActive(false);
            robotsPreview.Add(new List<GameObject>());
            robotsPreview[i].Add(robots[i]);
        }

        turns = 1;
    }

    void OnDestroy()
    {
        RobotBehaviour.OnClick -= new RobotBehaviour.ClickedOnRobot(SelectRobot);
        DestroyRobots();
    }

    void CreateRobots(int numberOfRobots)
    {
        int count = 0;
        while (transform.childCount > count)
        {
            robots.Add(transform.GetChild(count).gameObject);
            robots[count].GetComponent<RobotBehaviour>().freeTime = roundTime;
            count++;
        }
    }

    void DestroyRobots()
    {
        if (robots != null)
        {
            for (int i = 0; i < robots.Count; i++)
            {
                Destroy(robots[i]);
            }
            robots.Clear();
        }
    }

    public void PauseGame()
    {
        //put all robots into pausestate
        foreach (GameObject r in robots)
        {
            RobotBehaviour rb = r.GetComponent<RobotBehaviour>();
            rb.CurrentState.EnterPauseState();
            rb.freeTime = roundTime;
        }

        for (int i = 0; i < robots.Count; i++)
        {
            robotsMovingPreviews.Add(new GameObject());
            robotsMovingPreviews[i].name = "Robot moving trail";
            robotsMovingPreviews[i].SetActive(false);
            robotsPreview[i].Add(robots[i]);
        }
    }

    public void UnpauseGame()
    {
        //put all robots into play
        int index = 0;
        foreach (GameObject r in robots)
        {
            if (r == null)
            {
                Debug.Log("Null at " + index);
            }
            index++;
            //save the robots position,velocity, commands etc
            moves.Add(new Move(r, Turns, r.GetComponent<RobotBehaviour>().Commands));

            r.GetComponent<RobotBehaviour>().CurrentState.EnterPlayState();
        }
        turns++;

        for (int i = 0; i < robotsMovingPreviews.Count;)
        {
            Destroy(robotsMovingPreviews.Last());
            robotsMovingPreviews.Remove(robotsMovingPreviews.Last());
            robotsPreview[i].Clear();
        }
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
                robots[i].GetComponent<Rigidbody2D>().velocity = moves[turnIndex].Velocity;
            }
        }
    }

    void SelectRobot(GameObject r)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedRobot != r)
            {
                StopCoroutine(SetAndVisualizeTimeInput());
                for (int i = 0; i < robots.Count; i++)
                {
                    if (r == robots[i])
                    {
                        robotsMovingPreviews[selectedRobotIndex].SetActive(false);
                        selectedRobot = r;
                        selectedRobotIndex = i;
                        robotsMovingPreviews[selectedRobotIndex].SetActive(true);
                        Debug.Log("INDEX: " + i);
                        StartCoroutine(SetAndVisualizeTimeInput());
                        Debug.Log("Robot selected!");
                        break;
                    }
                }
                if (selectedRobot == null)
                {
                    Debug.Log("The selected robot is not know to the the TurnHandler, so therefore no commands can be given to it.");
                }
                StartCoroutine(SetAndVisualizeTimeInput());
            }
            else
            {
                Debug.Log("Robot already selected!");
            }
        }
    }

    IEnumerator SetAndVisualizeTimeInput()
    {

        if (cursorText == null)
        {
            //om den inte hittar, instansera istället!
            cursorText = GameObject.Find("CursorText").GetComponent<Text>();
        }
        float secondsPerDistance = 0.3f;
        RobotBehaviour selectRB = selectedRobot.GetComponent<RobotBehaviour>();
        Vector3 cursorPosition;
        Vector3 cursorScreenPosition;
        Vector3 deltaPosition;
        float deltaDistance, maxDeltaDistance;
        float remainingTimeForRobot;
        float previewInputTime, maxInputTime;
        float shockwaveLife = shockWavePrefab.GetComponent<ShockwaveBehaviour>().intendedLifetime;
        while (selectedRobot != null)
        {
            cursorPosition = Input.mousePosition;
            cursorScreenPosition = Camera.main.ScreenToWorldPoint(cursorPosition);

            deltaPosition = cursorScreenPosition - robotsPreview[selectedRobotIndex].Last().transform.position;

            deltaDistance = Mathf.Sqrt(Mathf.Pow(deltaPosition.x, 2) + Mathf.Pow(deltaPosition.y, 2));

            previewInputTime = secondsPerDistance * deltaDistance;
            remainingTimeForRobot = selectRB.freeTime - previewInputTime;

            if (selectedCommand == AvailableCommands.PushCommand)
            {
                maxInputTime = selectRB.freeTime - shockwaveLife;
            }
            else
            {
                maxInputTime = selectRB.freeTime;
            }
            maxDeltaDistance = maxInputTime / secondsPerDistance;

            if (previewInputTime <= maxInputTime)
            {
                timeInput = previewInputTime;
                cursorText.text = timeInput.ToString();
                cursorText.transform.position = cursorPosition;
            }
            else
            {
                timeInput = maxInputTime;
            }
            //else
            //{
            //    Vector3 normalizedCursorScreenPos = cursorScreenPosition.normalized;
            //    Vector3 maxPosition = robotsPreview[selectedRobotIndex].Last().transform.position + new Vector3(normalizedCursorScreenPos.x * maxDeltaDistance, normalizedCursorScreenPos.y * maxDeltaDistance);
            //    maxPosition = Camera.main.WorldToScreenPoint(maxPosition);
            //    cursorText.text = timeInput.ToString();
            //    cursorText.transform.position = maxPosition;
            //}
            cursorText.transform.position = cursorPosition;
            yield return new WaitForSeconds(0.0001f);
        }
        cursorText.text = "";
        yield return new WaitForSeconds(0.0001f);
    }

    void GiveCommandToSelectedRobot()
    {
        if (selectedRobot != null && !mouseButtonIsPressed)
        {
            //take the time for the command from the timetext
            RobotBehaviour rb = selectedRobot.GetComponent<RobotBehaviour>();
            if (timeInput > 0 && timeInput <= rb.freeTime)
            {
                Vector3 cursorPosition = Input.mousePosition;
                Vector3 cursorScreenPosition = Camera.main.ScreenToWorldPoint(cursorPosition);

                Command command = null, previewCommand = null;
                if (selectedCommand == AvailableCommands.MoveCommand)
                {
                    previewCommand = new MoveCommand(robotsPreview[selectedRobotIndex].Last(), cursorScreenPosition, timeInput, Turns);
                    command = new MoveCommand(selectedRobot, previewCommand as MoveCommand);
                    Debug.Log("MoveCommand Added!");
                }
                else if (selectedCommand == AvailableCommands.PushCommand && timeInput <= rb.freeTime - shockWavePrefab.GetComponent<ShockwaveBehaviour>().intendedLifetime)
                {
                    command = new PushCommand(selectedRobot, cursorScreenPosition, timeInput, Turns);
                    previewCommand = new PushCommand(robotsPreview[selectedRobotIndex].Last(), cursorScreenPosition, timeInput, Turns);
                    Debug.Log("PushCommand Added!");
                }
                else
                {
                    return;
                }

                latestTrail = new MovingTrail(previewCommand, timeInput, robotsPreview[selectedRobotIndex].Last().GetComponent<RobotBehaviour>().prevVelocity);
                latestTrail.TrailGameObject.transform.parent = robotsMovingPreviews[selectedRobotIndex].transform;
                robotsPreview[selectedRobotIndex].Add(latestTrail.Node);
                Debug.Log("command: " + command);
                rb.Commands.Add(command);
                rb.freeTime -= timeInput;
                timeInput = 0;
            }
        }
    }

    void Update()
    {
        ReactToUserInput();
    }

    IEnumerator PreviewRobotCommand()
    {
        yield return new WaitForSeconds(1f);
    }

    void GiveCommand(Command original)
    {
        Command givenCommand = null;
        if (original.GetType() == typeof(MoveCommand))
        {
            givenCommand = new MoveCommand(selectedRobot, original as MoveCommand);
        }
        else if(original.GetType() == typeof(PushCommand))
        {
            givenCommand = new PushCommand(selectedRobot, original as PushCommand);
        }
        selectedRobot.GetComponent<RobotBehaviour>().Commands.Add(givenCommand);
    }

    void ReactToUserInput()
    {

        if (Input.GetMouseButton(1))
        {
            GiveCommandToSelectedRobot();
            mouseButtonIsPressed = true;
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
        if (Input.GetMouseButtonUp(1))
        {
            mouseButtonIsPressed = false;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selectedRobot = null;
            timeInput = 0;
        }
    }

    public void Activate(bool active)
    {
        activated = active;
        if (active == true)
        { //visually indicate that this turnhandlers robots are now active
            //start taking events
            RobotBehaviour.OnClick += new RobotBehaviour.ClickedOnRobot(SelectRobot);
            //PushCommand.OnInstantiateShockWave += new PushCommand.InstantiateShockWave(InstantiateShockwave);

            foreach (GameObject r in robots)
            {
                r.GetComponent<RobotBehaviour>().shouldSendEvent = true;
            }
            enabled = true;
        }
        else
        {
            selectedRobot = null;
            timeInput = 0;
            RobotBehaviour.OnClick -= new RobotBehaviour.ClickedOnRobot(SelectRobot);

            for (int i = 0; i < robots.Count; i++)
            {
                robots[i].GetComponent<RobotBehaviour>().shouldSendEvent = false;
            }

            for (int i = 0; i < robotsMovingPreviews.Count; i++)
            {
                robotsMovingPreviews[i].SetActive(false);
            }
            enabled = false;
        }
    }

    public void ReplayLastTurn()
    {
        //save commando lists in robots where they are longer than 0
        //and put them in that robots oldCommands<List>
        foreach (GameObject r in robots)
        {
            if (r.GetComponent<RobotBehaviour>().Commands.Count > 0)
            {
                r.GetComponent<RobotBehaviour>().oldCommands = r.
                    GetComponent<RobotBehaviour>().Commands;
            }
        }
        //go through the last 8 moves and move each robot to their old position
        for (int i = moves.Count - 1; i > moves.Count - numberOfRobots - 1; i--)
        {

            Move m = moves[i];
            GameObject r = m.Robot;
            r.GetComponent<Rigidbody2D>().angularVelocity = m.AngularVelocity;
            r.GetComponent<Rigidbody2D>().velocity = m.Velocity;
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
}