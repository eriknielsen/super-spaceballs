using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class TurnHandlerBehaviour : MonoBehaviour
{

    public List<Move> moves;

    GameObject selectedCommandWheel;
    [SerializeField]
    GameObject shockWavePrefab;
    [SerializeField]
    GameObject commandWheelPrefab;  //Command selection buttons

    public int CurrentPlanTimeLeft { get; set; }
    public int Turns { get { return turns; } }
    public List<GameObject> Robots { get { return robots; } }

    [HideInInspector]
    public float roundTime;
    [HideInInspector]
    public float currentPlanTimeLeft; //the time left for this player to plan their move


    Text cursorText;

    int selectedRobotIndex;
    GameObject selectedRobot;
    Command.AvailableCommands selectedCommand;

    int turns;
    float timeInput;
    bool previewBallTrajectory = true;
    GameObject ball;
    PreviewMarker pm;
    Vector3 cursorPosition;
    Vector3 prevCursorPosition = Vector3.zero;
    Coroutine setAndDisplayTimeInput;
    Coroutine previewTrajectoryAndGiveRobotCommand;
    List<GameObject> movingPreviews;
    MovingTrail latestBallTrail;
    MovingTrail latestRobotTrail;
    List<List<MovingTrail>> ballMovingTrails;
    List<List<MovingTrail>> robotMovingTrails;
    List<GameObject> robots;
    GameObject directionPointer;
    ShockwaveConeScript swcs;
    LineRenderer commmandDirectionPointer;

    void Awake()
    {
        swcs = GameObject.Find("ShockwaveCone").GetComponent<ShockwaveConeScript>();
        commmandDirectionPointer = GameObject.Find("CommandDirectionPointer").GetComponent<LineRenderer>();
        //commmandDirectionPointer.gameObject.SetActive(false);
        //pm = GameObject.Find("PreviewMarker").GetComponent<PreviewMarker>();
        //ball = FindObjectOfType<Ball>().gameObject;
        selectedCommand = Command.AvailableCommands.None;
        moves = new List<Move>();
        robots = new List<GameObject>();
        FindRobots();
        movingPreviews = new List<GameObject>();
        robotMovingTrails = new List<List<MovingTrail>>();
        ballMovingTrails = new List<List<MovingTrail>>();
        for (int i = 0; i < robots.Count; i++)
        {
            movingPreviews.Add(new GameObject());
            movingPreviews[i].name = "Moving Previews";
            movingPreviews[i].SetActive(false);
            robotMovingTrails.Add(new List<MovingTrail>());
            ballMovingTrails.Add(new List<MovingTrail>());
        }
        turns = 1;

    }
    void Start()
    {
        if (cursorText == null)
        {
            cursorText = GameObject.Find("CursorText").GetComponent<Text>();
            cursorText.text = "";
        }
    }
    void FindRobots()
    {
        for (int i = 0; transform.childCount > i; i++)
        {
            robots.Add(transform.GetChild(i).gameObject);
            robots[i].GetComponent<RobotBehaviour>().freeTime = roundTime;
        }
    }

    public void PauseGame()
    {
        foreach (GameObject robot in robots)
        { //put all robots into pausestate
            RobotBehaviour robotBehaviour = robot.GetComponent<RobotBehaviour>();
            robotBehaviour.CurrentState.EnterPauseState();
            robotBehaviour.freeTime = roundTime;
        }
        DisableMovingPreviews();
    }

    public void UnpauseGame()
    {
        foreach (GameObject robot in robots) //put all robots into play
        {
            //save the robots position,velocity, commands etc
            moves.Add(new Move(robot, Turns, robot.GetComponent<RobotBehaviour>().Commands));

            robot.GetComponent<RobotBehaviour>().CurrentState.EnterPlayState();
        }
        turns++;

        RemoveAllMovingTrails();
    }

    void RemoveAllMovingTrails()
    {
        for (int i = 0; i < robotMovingTrails.Count; i++)
        {
            for (int j = 0; j < robotMovingTrails[i].Count; j++)
            {
                robotMovingTrails[i][j].DestroyTrail();
            }
            robotMovingTrails[i].Clear();
        }
        for (int i = 0; i < ballMovingTrails.Count; i++)
        {
            for (int j = 0; j < ballMovingTrails[i].Count; j++)
            {
                ballMovingTrails[i][j].DestroyTrail();
            }
            ballMovingTrails[i].Clear();
        }
    }

    void DisableMovingPreviews()
    {
        for (int i = 0; i < robots.Count; i++)
        {
            movingPreviews[i].SetActive(false);
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

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    void SelectRobot(GameObject robot)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedRobot != robot)
            {
                THDeselectRobot();
                for (int i = 0; i < robots.Count; i++)
                {
                    if (robot == robots[i])
                    {
                        selectedCommandWheel = Instantiate(commandWheelPrefab, new Vector3(robots[i].transform.position.x, robots[i].transform.position.y, robots[i].transform.position.z - 1), Quaternion.identity); //Command selection buttons
                        movingPreviews[selectedRobotIndex].SetActive(false);
                        selectedRobot = robot;
                        selectedRobotIndex = i;
                        break;
                    }
                }
            }
        }
    }

    IEnumerator SetAndDisplayTimeInput()
    {
        float secondsPerDistance = 0.3f;
        RobotBehaviour selectedRB;
        Vector3 cursorPosition;
        Vector3 cursorScreenPosition;
        Vector3 deltaPosition;
        float deltaDistance;
        float previewInputTime, maxInputTime;
        float shockwaveLife = shockWavePrefab.GetComponent<ShockwaveBehaviour>().intendedLifetime;

        Cursor.visible = false;

        while (selectedRobot != null && selectedCommand != Command.AvailableCommands.None)
        {
            selectedRB = selectedRobot.GetComponent<RobotBehaviour>();
            cursorPosition = Input.mousePosition;
            cursorScreenPosition = Camera.main.ScreenToWorldPoint(cursorPosition);

            if (robotMovingTrails[selectedRobotIndex].Count > 0)
            {
                deltaPosition = cursorScreenPosition - robotMovingTrails[selectedRobotIndex].Last().Node.transform.position;
            }
            else
            {
                deltaPosition = cursorScreenPosition - selectedRobot.transform.position;
            }
            deltaDistance = Mathf.Sqrt(Mathf.Pow(deltaPosition.x, 2) + Mathf.Pow(deltaPosition.y, 2));

            previewInputTime = secondsPerDistance * deltaDistance;

            if (selectedCommand == Command.AvailableCommands.Push)
            {
                //add a bit of time to make sure the shockwave dies before pausing
                maxInputTime = selectedRB.freeTime - (shockwaveLife + 0.1f);
                if (maxInputTime < 0)
                {
                    maxInputTime = 0;
                }
            }
            else
            {
                maxInputTime = selectedRB.freeTime;
            }

            if (previewInputTime <= maxInputTime)
            {
                timeInput = previewInputTime;
                //                if (Input.GetKeyDown(KeyCode.Space)) //Code related to other preview marking method
                //                    pm.simulatepath2(selectedRobot.transform.position, selectedRobot.GetComponent<Rigidbody2D>().velocity, timeInput, cursorScreenPosition);
            }
            else
            {
                timeInput = maxInputTime;
            }
            cursorText.text = System.Math.Round(timeInput, 2).ToString();
            cursorText.transform.position = cursorPosition;

            yield return new WaitForSeconds(0.005f);
        }
    }

    IEnumerator PreviewTrajectoryAndGiveRobotCommand()
    {
        Vector3 prevCursorPosition = Vector3.zero;
        Vector3 cursorPosition = Input.mousePosition;
        Vector3 cursorScreenPosition = Camera.main.ScreenToWorldPoint(cursorPosition);
        Command previewCommand = null;

        GameObject lastRobot = null;
        GameObject previewRobot = null;
        float timeBetweenPreivews = 0.1f;
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        Command.AvailableCommands prevSelectedCommand = selectedCommand;

        timer.Start();
        while (selectedRobot != null)
        {
            cursorPosition = Input.mousePosition;
            cursorScreenPosition = Camera.main.ScreenToWorldPoint(cursorPosition);
            if (timeInput <= selectedRobot.GetComponent<RobotBehaviour>().freeTime && timer.Elapsed.TotalSeconds > timeBetweenPreivews)
            {
                if (prevCursorPosition != cursorPosition || prevSelectedCommand != selectedCommand || RevertCommand() || lastRobot != selectedRobot)
                {
                    timer.Reset();
                    timer.Start();
                    DestroyLatestPreviewTrail();
                    if (robotMovingTrails[selectedRobotIndex].Count > 0)
                    {
                        previewRobot = robotMovingTrails[selectedRobotIndex].Last().Node;
                    }
                    else
                    {
                        previewRobot = selectedRobot;
                    }
                    cursorPosition = Input.mousePosition;

                    cursorScreenPosition = Camera.main.ScreenToWorldPoint(cursorPosition);

                    if (selectedCommand == Command.AvailableCommands.Move)
                    {
                        previewCommand = new MoveCommand(previewRobot, cursorScreenPosition, timeInput, Turns);

                    }
                    else if (selectedCommand == Command.AvailableCommands.Push && timeInput <= selectedRobot.GetComponent<RobotBehaviour>().freeTime - shockWavePrefab.GetComponent<ShockwaveBehaviour>().intendedLifetime)
                    {
                        previewCommand = new PushCommand(previewRobot, cursorScreenPosition, timeInput, Turns);
                    }
                    else
                    {
                        Debug.Log("No command selected!");
                    }
                    latestRobotTrail = new MovingTrail(previewCommand, timeInput, previewRobot.GetComponent<RobotBehaviour>().prevVelocity);
                    if (previewCommand.GetType() == typeof(PushCommand))
                    {
                        //GameObject.Find("ShockwaveCone").GetComponent<ShockwaveConeScript>().f//(cursorScreenPosition, previewRobot);
                        swcs.SetPositions(cursorScreenPosition, previewRobot.transform.position);
                    }
                }
                if (previewRobot != null)
                {
                    commmandDirectionPointer.SetPosition(0, previewRobot.transform.position);
                    commmandDirectionPointer.SetPosition(1, new Vector3(cursorScreenPosition.x, cursorScreenPosition.y, commmandDirectionPointer.GetPosition(1).z));
                }
                if (Input.GetMouseButtonDown(1) && latestRobotTrail != null)
                {
                    latestRobotTrail.TrailGameObject.transform.parent = movingPreviews[selectedRobotIndex].transform;
                    robotMovingTrails[selectedRobotIndex].Add(latestRobotTrail);
                    latestRobotTrail = null;

                    if (previewCommand.GetType() == typeof(PushCommand))
                    {
                        GameObject swcsPreview = Instantiate(swcs.gameObject) as GameObject;
                        swcsPreview.transform.parent = robotMovingTrails[selectedRobotIndex].Last().TrailGameObject.transform;
                        swcsPreview.name = "Push Command Cone";
                    }

                    GiveRobotCommand(previewCommand);
                }
            }
            if (selectedRobot != lastRobot)
            {
                DestroyLatestPreviewTrail();
            }
            prevCursorPosition = cursorPosition;
            prevSelectedCommand = selectedCommand;
            lastRobot = selectedRobot;
            yield return new WaitForSeconds(0.005f);
        }
        DestroyLatestPreviewTrail();
    }

    bool RevertCommand()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (robotMovingTrails[selectedRobotIndex].Count > 0)
            {
                robotMovingTrails[selectedRobotIndex].Last().DestroyTrail();
                robotMovingTrails[selectedRobotIndex].RemoveAt(robotMovingTrails[selectedRobotIndex].Count - 1);
            }
            RobotBehaviour selectedRobotBehaviour = robots[selectedRobotIndex].GetComponent<RobotBehaviour>();
            if (selectedRobotBehaviour.Commands.Count > 0)
            {
                selectedRobotBehaviour.freeTime += selectedRobotBehaviour.Commands.Last().lifeDuration;
                selectedRobotBehaviour.Commands.RemoveAt(selectedRobotBehaviour.Commands.Count - 1);
            }
            return true;
        }
        return false;
    }

    void DestroyLatestPreviewTrail()
    {
        if (latestRobotTrail != null)
        {
            Destroy(latestRobotTrail.TrailGameObject);
        }
        latestRobotTrail = null;
    }

    void GiveRobotCommand(Command command)
    {
        Command givenCommand = null;
        if (command != null)
        {
            if (command.GetType() != typeof(NoneCommand))
            {
                if (command.GetType() == typeof(MoveCommand))
                {
                    givenCommand = new MoveCommand(selectedRobot, command as MoveCommand);
                }
                else if (command.GetType() == typeof(PushCommand))
                {
                    PushCommand pushCommand = command as PushCommand;
                    float previewDuration = pushCommand.LifeDuration;
                    givenCommand = new PushCommand(selectedRobot, command as PushCommand, previewDuration);
                }
                selectedRobot.GetComponent<RobotBehaviour>().Commands.Add(givenCommand);

                selectedRobot.GetComponent<RobotBehaviour>().freeTime -= command.LifeDuration;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            THSelectCommand(Command.AvailableCommands.Move);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            THSelectCommand(Command.AvailableCommands.Push);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            THDeselectRobot();
        }

        if (previewBallTrajectory)
        { //Lots of dead code here
            while (selectedRobot != null && ball != null)
            {
                cursorPosition = Input.mousePosition;
                if (prevCursorPosition != cursorPosition)
                {

                    if (ballMovingTrails != null && ballMovingTrails[selectedRobotIndex].Count > 0)
                    {
                        Destroy(ballMovingTrails[selectedRobotIndex].First().TrailGameObject);
                        ballMovingTrails[selectedRobotIndex].Clear();
                    }
                    MoveCommand emptyMoveCommand = new MoveCommand(ball, Vector2.zero, 0, turns);
                    latestBallTrail = new MovingTrail(emptyMoveCommand, timeInput, ball.GetComponent<Ball>().PreviousVelocity);
                    ballMovingTrails[selectedRobotIndex].Add(latestBallTrail);
                }
                prevCursorPosition = cursorPosition;
            }
        }
    }
    //TH prefix to indicate that these are passed through PlayBehaviour
    public void THSelectCommand(Command.AvailableCommands command)
    {
        commmandDirectionPointer.gameObject.SetActive(true);
        selectedCommand = command;
        if (selectedCommand != Command.AvailableCommands.None)
        { //StartCoroutine(PreviewBallTrajectory());?
            movingPreviews[selectedRobotIndex].SetActive(true);
            if (selectedCommand != Command.AvailableCommands.Push)
            {
                swcs.DeActivateSprite();
            }
            setAndDisplayTimeInput = StartCoroutine(SetAndDisplayTimeInput());
            previewTrajectoryAndGiveRobotCommand = StartCoroutine(PreviewTrajectoryAndGiveRobotCommand());

        }
        else
        {
            movingPreviews[selectedRobotIndex].SetActive(false);
            HideCursorText();
            DestroyPreviewTrails();
            StopCoroutineIfNotNull(previewTrajectoryAndGiveRobotCommand);
        }
    }

    public void THDeselectRobot()
    {
        commmandDirectionPointer.gameObject.SetActive(false);
        swcs.DeActivateSprite();
        selectedRobot = null;
        timeInput = 0;
        selectedCommand = Command.AvailableCommands.None;
        HideCursorText();
        DestroyPreviewTrails();
        StopCoroutineIfNotNull(previewTrajectoryAndGiveRobotCommand);
        if (selectedCommandWheel != null)
        {
            Destroy(selectedCommandWheel);
        }
        if (movingPreviews.Count > 0)
        {
            movingPreviews[selectedRobotIndex].SetActive(false);
        }
    }

    void HideCursorText()
    {
        cursorText.text = "";
        Cursor.visible = true;
        StopCoroutineIfNotNull(setAndDisplayTimeInput);
    }

    void StopCoroutineIfNotNull(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    void DestroyPreviewTrails()
    {
        if (latestRobotTrail != null)
        {
            Destroy(latestRobotTrail.TrailGameObject);
        }
        latestRobotTrail = null;
    }

    public void Activate(bool activate)
    {
        if (activate == true)
        {
            //visually indicate that this turnhandlers robots are now active
            //start taking events
            RobotBehaviour.OnClick += new RobotBehaviour.ClickedOnRobot(SelectRobot);
            foreach (GameObject robot in robots)
            {
                robot.GetComponent<RobotBehaviour>().shouldSendEvent = true;
                robot.GetComponent<HaloScript>().enabled = true;

            }

        }
        else
        {
            THDeselectRobot();
            RobotBehaviour.OnClick -= new RobotBehaviour.ClickedOnRobot(SelectRobot);
            GameObject.Find("ShockwaveCone").GetComponent<ShockwaveConeScript>().DeActivateSprite();
            for (int i = 0; i < robots.Count; i++)
            {
                robots[i].GetComponent<RobotBehaviour>().shouldSendEvent = false;
                robots[i].GetComponent<HaloScript>().enabled = false;
            }

            for (int i = 0; i < movingPreviews.Count; i++)
            {
                movingPreviews[i].SetActive(false);
            }
        }
    }

    public void ReplayLastTurn()
    { //save commando lists in robots where they are longer than 0 and put them in that robots oldCommands<List>
        foreach (GameObject robot in robots)
        {
            if (robot.GetComponent<RobotBehaviour>().Commands.Count > 0)
            {
                robot.GetComponent<RobotBehaviour>().oldCommands = robot.GetComponent<RobotBehaviour>().Commands;
            }
        }
        //go through the last 8 moves and move each robot to their old position
        for (int i = moves.Count - 1; i > moves.Count - Robots.Count - 1; i--)
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
        foreach (GameObject robot in robots)
        {
            if (robot.GetComponent<RobotBehaviour>().oldCommands.Count > 0)
            {
                RobotBehaviour robotBehaviour = robot.GetComponent<RobotBehaviour>();
                robotBehaviour.Commands = robotBehaviour.oldCommands;
                robotBehaviour.oldCommands.Clear();
            }
        }
    }
}