using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.Diagnostics;

public class TurnHandlerBehaviour : MonoBehaviour
{

<<<<<<< HEAD
=======
public class TurnHandlerBehaviour : MonoBehaviour {
  
	public List<Move> moves;
>>>>>>> origin/master
    [SerializeField]
    GameObject shockWavePrefab;
    [HideInInspector]
    public float roundTime;

    private Text cursorText;
	private GameObject selectedRobot;
    private int selectedRobotIndex;
    private Command.AvailableCommands selectedCommand;
	private bool activated = false;
	private bool mouseButtonIsPressed = false;
	private float timeInput;
	private List<GameObject> robotsMovingPreviews;
	private List<List<GameObject>> robotsPreview;
	private MovingTrail latestTrail;

    List<GameObject> robots;
    int turns;
    bool mouseButtonIsPressed = false;
    BoxCollider2D bc2D;
    float timeInput;
    List<GameObject> robotsMovingPreviews;
    List<List<MovingTrail>> robotMovingTrails;
    MovingTrail latestTrail;

    bool gameIsPaused = true;
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
        FindRobots(numberOfRobots);
        robotsMovingPreviews = new List<GameObject>();
        robotMovingTrails = new List<List<MovingTrail>>();
        UnityEngine.Debug.Log("Robot numbers: " + robots.Count);
        for (int i = 0; i < robots.Count; i++)
        {
            robotsMovingPreviews.Add(new GameObject());
            robotsMovingPreviews[i].name = "Robot moving preview";
            robotsMovingPreviews[i].SetActive(false);
            robotMovingTrails.Add(new List<MovingTrail>());
            robotMovingTrails[i].Add(new MovingTrail(new MoveCommand(robots[i], robots[i].transform.position, 0f, turns), 0f, Vector2.zero));
            robotMovingTrails[i].Last().TrailGameObject.transform.parent = robotsMovingPreviews[i].transform;
        }
        turns = 1;
    }
	void FindRobots(){
		for (int i = 0; transform.childCount > i; i++){
            robots.Add(transform.GetChild(i).gameObject);
            robots[i].GetComponent<RobotBehaviour>().freeTime = roundTime;
        }
    }

	void Update(){
		if (Input.GetMouseButton(1)){	//React to user input (OLD; REMOVE WHEN BUTTONS ARE DONE)
			GiveCommandToSelectedRobot();
			mouseButtonIsPressed = true;
		}
		if (Input.GetKeyDown(KeyCode.Z)){
			Debug.Log("movecommand chosen");
			selectedCommand = Command.AvailableCommands.Move;
		}
		if (Input.GetKeyDown(KeyCode.X)){
			Debug.Log("pushcommand chosen");
			selectedCommand = Command.AvailableCommands.Push;
		}
		if (Input.GetMouseButtonUp(1)){
			mouseButtonIsPressed = false;
		}
		if (Input.GetKeyDown(KeyCode.Escape)){
			selectedRobot = null;
			timeInput = 0;
		}
	}

    public void PauseGame(){
		foreach (GameObject r in robots){ //put all robots into pausestate
            RobotBehaviour rb = r.GetComponent<RobotBehaviour>();
            rb.CurrentState.EnterPauseState();
            rb.freeTime = roundTime;
        }
        for (int i = 0; i < robots.Count; i++){
            robotsMovingPreviews.Add(new GameObject());
            robotsMovingPreviews[i].name = "Robot moving preview";
            robotsMovingPreviews[i].SetActive(false);
            robotMovingTrails[i].Add(new MovingTrail(new MoveCommand(robots[i], robots[i].transform.position, 0f, turns), 0f, Vector2.zero));
            robotMovingTrails[i].Last().TrailGameObject.transform.parent = robotsMovingPreviews[i].transform.parent;
        }
    }

    public void UnpauseGame()
    {
        gameIsPaused = false;
        //put all robots into play
        int index = 0;
        foreach (GameObject r in robots)
        {
            if (r == null)
            {
                UnityEngine.Debug.Log("Null at " + index);
            }
            index++;
            //save the robots position,velocity, commands etc
            moves.Add(new Move(r, Turns, r.GetComponent<RobotBehaviour>().Commands));

            r.GetComponent<RobotBehaviour>().CurrentState.EnterPlayState();
        }
        turns++;

        for (int i = 0; i < robotsMovingPreviews.Count;){
            Destroy(robotsMovingPreviews.Last());
            robotsMovingPreviews.Remove(robotsMovingPreviews.Last());
            robotMovingTrails[i].Clear();
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

    void SelectRobot(GameObject r)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedRobot != r)
            {
                StopCoroutine(SetAndDisplayTimeInput());
                for (int i = 0; i < robots.Count; i++)
                {
                    if (r == robots[i])
                    {
                        robotsMovingPreviews[selectedRobotIndex].SetActive(false);
                        selectedRobot = r;
                        selectedRobotIndex = i;
                        robotsMovingPreviews[selectedRobotIndex].SetActive(true);
                        UnityEngine.Debug.Log("INDEX: " + i);
                        StartCoroutine(SetAndDisplayTimeInput());
                        UnityEngine.Debug.Log("Robot selected!");
                        break;
                    }
                }

                if (selectedRobot == null)
                {
                    UnityEngine.Debug.Log("The selected robot is not know to the the TurnHandler, so therefore no commands can be given to it.");
                }
                StartCoroutine(SetAndDisplayTimeInput());
            }
            else
            {
                UnityEngine.Debug.Log("Robot already selected!");
            }
        }
    }

    IEnumerator SetAndDisplayTimeInput()
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

            deltaPosition = cursorScreenPosition - robotMovingTrails[selectedRobotIndex].Last().Node.transform.position;

            deltaDistance = Mathf.Sqrt(Mathf.Pow(deltaPosition.x, 2) + Mathf.Pow(deltaPosition.y, 2));

            previewInputTime = secondsPerDistance * deltaDistance;
            remainingTimeForRobot = selectRB.freeTime;

            if (selectedCommand == AvailableCommands.PushCommand)
            {
                maxInputTime = selectRB.freeTime - shockwaveLife;
            }
            else {
                maxInputTime = selectRB.freeTime;
            }

            if (previewInputTime <= maxInputTime)
            {
                timeInput = previewInputTime;
            }
            else
            {
                timeInput = remainingTimeForRobot;
            }
            cursorText.text = timeInput.ToString();
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
                    previewCommand = new MoveCommand(robotMovingTrails[selectedRobotIndex].Last().Node, cursorScreenPosition, timeInput, Turns);
                    command = new MoveCommand(selectedRobot, previewCommand as MoveCommand);
                    UnityEngine.Debug.Log("MoveCommand Added!");
                }
				else if (selectedCommand == Command.AvailableCommands.Push && timeInput <= rb.freeTime - shockWavePrefab.GetComponent<ShockwaveBehaviour>().intendedLifetime){
                    command = new PushCommand(selectedRobot, cursorScreenPosition, timeInput, Turns);
                    previewCommand = new PushCommand(robotMovingTrails[selectedRobotIndex].Last().Node, cursorScreenPosition, timeInput, Turns);
                    UnityEngine.Debug.Log("PushCommand Added!");
                }
                else
                {
                    return;
                }
                latestTrail = new MovingTrail(previewCommand, timeInput, robotMovingTrails[selectedRobotIndex].Last().Node.GetComponent<RobotBehaviour>().prevVelocity);
                latestTrail.TrailGameObject.transform.parent = robotsMovingPreviews[selectedRobotIndex].transform;
                robotMovingTrails[selectedRobotIndex].Add(latestTrail);
                rb.Commands.Add(command);
                rb.freeTime -= timeInput;
                timeInput = 0;
            }
        }
    }

<<<<<<< HEAD
    IEnumerator PreviewAndGiveRobotCommand()
    {
        Vector3 prevCursorPosition = Vector3.zero;
        Vector3 cursorPosition = Input.mousePosition;
        Vector3 cursorScreenPosition = Camera.main.ScreenToWorldPoint(cursorPosition);
        Command previewCommand = null;
        Stopwatch previewTimer = new Stopwatch();
        previewTimer.Start();
        float timeBetweenPreivews = 0.5f;
        while (gameIsPaused)
        {
            cursorPosition = Input.mousePosition;
            cursorScreenPosition = Camera.main.ScreenToWorldPoint(cursorPosition);
            if (timeInput > 0 && timeInput <= selectedRobot.GetComponent<RobotBehaviour>().freeTime)
            {
                if (previewTimer.Elapsed.TotalSeconds > timeBetweenPreivews && prevCursorPosition != cursorPosition)
                {
                    Destroy(robotMovingTrails[selectedRobotIndex].Last().TrailGameObject);
                    robotMovingTrails[selectedRobotIndex].Remove(robotMovingTrails[selectedRobotIndex].Last());

                    cursorPosition = Input.mousePosition;
                    cursorScreenPosition = Camera.main.ScreenToWorldPoint(cursorPosition);

                    if (selectedCommand == AvailableCommands.MoveCommand)
                    {
                        previewCommand = new MoveCommand(robotMovingTrails[selectedRobotIndex].Last().Node, cursorScreenPosition, timeInput, Turns);
                    }
                    else if (selectedCommand == AvailableCommands.PushCommand && timeInput <= selectedRobot.GetComponent<RobotBehaviour>().freeTime - shockWavePrefab.GetComponent<ShockwaveBehaviour>().intendedLifetime)
                    {
                        previewCommand = new PushCommand(robotMovingTrails[selectedRobotIndex].Last().Node, cursorScreenPosition, timeInput, Turns);
                    }

                    latestTrail = new MovingTrail(previewCommand, timeInput, robotMovingTrails[selectedRobotIndex].Last().Node.GetComponent<RobotBehaviour>().prevVelocity);
                    latestTrail.TrailGameObject.transform.parent = robotsMovingPreviews[selectedRobotIndex].transform;
                    robotMovingTrails[selectedRobotIndex].Add(latestTrail);
                    previewTimer.Reset();
                }
            }
            prevCursorPosition = cursorPosition;
            yield return new WaitForSeconds(0.0000001f);
        }
    }

    void GiveRobotCommand(Command command)
    {
        Command givenCommand = null;
        if (command.GetType() == typeof(MoveCommand))
        {
            givenCommand = new MoveCommand(selectedRobot, command as MoveCommand);
        }
        else if (command.GetType() == typeof(PushCommand))
        {
            givenCommand = new PushCommand(selectedRobot, command as PushCommand);
        }
        selectedRobot.GetComponent<RobotBehaviour>().Commands.Add(givenCommand);
    }

    void Update()
    {
        ReactToUserInput();
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
            UnityEngine.Debug.Log("movecommand chosen");
            selectedCommand = AvailableCommands.MoveCommand;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            UnityEngine.Debug.Log("pushcommand chosen");
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

	public void SelectCommand(Command.AvailableCommands command){  //NEEDS PUBLIC ACCESS (STATIC?)
		selectedCommand = command;
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