using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class TurnHandlerBehaviour : MonoBehaviour
{
    [SerializeField]
    RobotBehaviour previewRobotPrefab;
	public List<Move> moves;
    [SerializeField]
    GameObject shockWavePrefab;
	[SerializeField]
	GameObject commandWheelPrefab;  //Command selection buttons
	GameObject selectedCommandWheel;
    [HideInInspector]
    public float roundTime;

    private Text cursorText;
	private GameObject selectedRobot;
    private int selectedRobotIndex;
    private Command.AvailableCommands selectedCommand;


    private float timeInput;
    private List<GameObject> movingPreviews;
    private string movingPreviewsName = "Moving Previews";
	private MovingTrail latestRobotTrail;
    private MovingTrail latestBallTrail;
    private List<List<MovingTrail>> robotMovingTrails;
    private List<List<MovingTrail>> ballMovingTrails;
    private GameObject ball;
    private PreviewMarker pm;
    List<GameObject> robots;
    int turns;

    public int Turns
    {
        get { return turns; }
    }
	
    public List<GameObject> Robots
    {
        get { return robots; }
    }
    void Start()
    {
        //pm = GameObject.Find("PreviewMarker").GetComponent<PreviewMarker>();
        ball = FindObjectOfType<Ball>().gameObject;
		selectedCommand = Command.AvailableCommands.Move;
		moves = new List<Move>();
		robots = new List<GameObject>();
        FindRobots();
        movingPreviews = new List<GameObject>();
		robotMovingTrails = new List<List<MovingTrail>>();
        ballMovingTrails = new List<List<MovingTrail>>();
		for (int i = 0; i < robots.Count; i++)
		{
			movingPreviews.Add(new GameObject());
			movingPreviews[i].name = movingPreviewsName;
			movingPreviews[i].SetActive(false);
			robotMovingTrails.Add(new List<MovingTrail>());
            ballMovingTrails.Add(new List<MovingTrail>());
        }
        turns = 1;
        if(cursorText == null)
        {
            cursorText = GameObject.Find("CursorText").GetComponent<Text>();
			cursorText.text = "";
        }
    }

	void FindRobots(){
		for (int i = 0; transform.childCount > i; i++){
			robots.Add(transform.GetChild(i).gameObject);
			robots[i].GetComponent<RobotBehaviour>().freeTime = roundTime;
		}
	}
	
    public void PauseGame(){
		foreach (GameObject robot in robots){ //put all robots into pausestate
			RobotBehaviour robotBehaviour = robot.GetComponent<RobotBehaviour>();
            robotBehaviour.CurrentState.EnterPauseState();
            robotBehaviour.freeTime = roundTime;
        }

        DisableMovingPreviews();
    }

    public void UnpauseGame()
    {
        
        //put all robots into play
        foreach (GameObject robot in robots)
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

    void UndoLastMove(){
        if (Turns > 0){
            //reset all robots to the previous' move's position
            int turnIndex = 0;
            for (int i = Turns - 1; i < Turns * robots.Count; i++){
                turnIndex = (Turns - 1) * robots.Count + i;
                robots[i].transform.position = moves[turnIndex].position;
                robots[i].GetComponent<Rigidbody2D>().velocity = moves[turnIndex].Velocity;
            }
        }
    }

    void SelectRobot(GameObject robot)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedRobot != robot)
			{
				THDeselectRobot ();
                for (int i = 0; i < robots.Count; i++)
                {
                    if (robot == robots[i])
                    {
						selectedCommandWheel = Instantiate (commandWheelPrefab, new Vector3(robots[i].transform.position.x, robots[i].transform.position.y, robots[i].transform.position.z-1), Quaternion.identity); //Command selection buttons
						movingPreviews[selectedRobotIndex].SetActive(false);
                        selectedRobot = robot;
                        robot.GetComponent<ParticleSystem>().Emit(15);
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
        float remainingTimeForRobot;
        float previewInputTime, maxInputTime;
        float shockwaveLife = shockWavePrefab.GetComponent<ShockwaveBehaviour>().intendedLifetime;
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
            remainingTimeForRobot = selectedRB.freeTime;

            if (selectedCommand == Command.AvailableCommands.Push)
            {
                maxInputTime = selectedRB.freeTime - shockwaveLife;
                if(maxInputTime < 0)
                {
                    maxInputTime = 0;
                }
            }
            else {
                maxInputTime = selectedRB.freeTime;
            }

            if (previewInputTime <= maxInputTime)
            {
                timeInput = previewInputTime;
//                if (Input.GetKeyDown(KeyCode.Space))
//                    pm.simulatepath2(selectedRobot.transform.position, selectedRobot.GetComponent<Rigidbody2D>().velocity, timeInput, cursorScreenPosition);
            }
            else
            {
                timeInput = maxInputTime;
            }
            cursorText.text = timeInput.ToString();
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

        GameObject previewRobot;
        float timeBetweenPreivews = 0.1f;
		System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        Command.AvailableCommands prevSelectedCommand = selectedCommand;

        timer.Start();
        while (selectedRobot != null)
        {
            cursorPosition = Input.mousePosition;
            cursorScreenPosition = Camera.main.ScreenToWorldPoint(cursorPosition);
            if (timeInput > 0 && timeInput <= selectedRobot.GetComponent<RobotBehaviour>().freeTime && timer.Elapsed.TotalSeconds > timeBetweenPreivews)
            {
                if (prevCursorPosition != cursorPosition || prevSelectedCommand != selectedCommand)
                {
                    timer.Reset();
                    timer.Start();
					DestroyPreviewTrails();
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
                        UnityEngine.Debug.Log("No command selected!");
                    }
                    latestRobotTrail = new MovingTrail(previewCommand, timeInput, previewRobot.GetComponent<RobotBehaviour>().prevVelocity);
                }
                if (Input.GetMouseButton(1) && latestRobotTrail != null)
                {
                    latestRobotTrail.TrailGameObject.transform.parent = movingPreviews[selectedRobotIndex].transform;
                    robotMovingTrails[selectedRobotIndex].Add(latestRobotTrail);
                    latestRobotTrail = null;
                    GiveRobotCommand(previewCommand);
                }
            }
            prevCursorPosition = cursorPosition;
            prevSelectedCommand = selectedCommand;
            yield return new WaitForSeconds(0.005f);
        }
        DestroyPreviewTrails();
    }

    IEnumerator PreviewBallTrajectory()
    {
        Vector3 prevCursorPosition = Vector3.zero;
        Vector3 cursorPosition;
        Vector3 cursorScreenPosition;
        
        while (selectedRobot != null && ball != null)
        {
            cursorPosition = Input.mousePosition;
            cursorScreenPosition = Camera.main.ScreenToWorldPoint(cursorPosition);
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
            yield return new WaitForSeconds(0.005f);
        }
    }

	void DestroyPreviewTrails(){
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
                UnityEngine.Debug.Log("Given command: " + givenCommand);
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
    }
	//TH prefix to indicate that these are passed through PlayBehaviour
	public void THSelectCommand(Command.AvailableCommands command)
	{
		selectedCommand = command;
		if (selectedCommand != Command.AvailableCommands.None)
		{
			movingPreviews[selectedRobotIndex].SetActive (true);
			StartCoroutine(SetAndDisplayTimeInput ());
			StartCoroutine(PreviewTrajectoryAndGiveRobotCommand ());
            //StartCoroutine(PreviewBallTrajectory());
		}
		else
		{
			movingPreviews[selectedRobotIndex].SetActive (false);
			StopAllCoroutines ();
			DestroyPreviewTrails();
			cursorText.text = "";

		}
	}

	public void THDeselectRobot()
	{
		selectedRobot = null;
		timeInput = 0;
		selectedCommand = Command.AvailableCommands.None;
		StopAllCoroutines ();
		DestroyPreviewTrails ();
		cursorText.text = "";
        //		StopCoroutine(SetAndDisplayTimeInput());
        //		StopCoroutine(PreviewAndGiveRobotCommand());
        if (selectedCommandWheel != null)
        {
            Destroy(selectedCommandWheel);
        }
        if (movingPreviews.Count > 0)
        {
            movingPreviews[selectedRobotIndex].SetActive(false);
        }
	}

    public void Activate(bool active)
    {
      
        if (active == true)
        {
            //visually indicate that this turnhandlers robots are now active
            //start taking events
            RobotBehaviour.OnClick += new RobotBehaviour.ClickedOnRobot(SelectRobot);
			foreach (GameObject robot in robots)
            {
                robot.GetComponent<RobotBehaviour>().shouldSendEvent = true;
            }
            
        }
        else
        {
			THDeselectRobot ();
            RobotBehaviour.OnClick -= new RobotBehaviour.ClickedOnRobot(SelectRobot);

            for (int i = 0; i < robots.Count; i++)
            {
                robots[i].GetComponent<RobotBehaviour>().shouldSendEvent = false;
            }

            for (int i = 0; i < movingPreviews.Count; i++)
            {
                movingPreviews[i].SetActive(false);
            }
        }
    }

    public void ReplayLastTurn()
    {
        //save commando lists in robots where they are longer than 0
        //and put them in that robots oldCommands<List>
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
				robotBehaviour.commands = robotBehaviour.oldCommands;
				robotBehaviour.oldCommands.Clear();
            }
        }
    }	
}