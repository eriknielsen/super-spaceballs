using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class TurnHandlerBehaviour : MonoBehaviour {

	[HideInInspector]
	public float roundTime;
	public List<Move> moves;

    [SerializeField]
	private GameObject shockWavePrefab;

    private Text cursorText;

    private GameObject selectedRobot;
    private int selectedRobotIndex;

	private int turns;
	public int Turns { get { return turns; } }
	private List<GameObject> robots;
	public List<GameObject> Robots { get { return robots; } }
	private bool mouseButtonIsPressed = false;
	private float timeInput;
	private List<GameObject> robotsMovingPreviews;
	private List<List<GameObject>> robotsPreview;
	private MovingTrail latestTrail;
	private bool activated = false;

	public Command.AvailableCommands selectedCommand;

	public void SetSelectedCommand(Command.AvailableCommands inputCommand){ //NEEDS PUBLIC ACCESS; STATIC?
		selectedCommand = inputCommand;
	}

    void Awake(){
        moves = new List<Move>();
        robots = new List<GameObject>();
        FindRobots();
        robotsMovingPreviews = new List<GameObject>();
        robotsPreview = new List<List<GameObject>>();

        for (int i = 0; i < robots.Count; i++){
            robotsMovingPreviews.Add(new GameObject());
            robotsMovingPreviews[i].name = "Robot moving trail";
            robotsMovingPreviews[i].SetActive(false);
            robotsPreview.Add(new List<GameObject>());
            robotsPreview[i].Add(robots[i]);
        }
        turns = 1;
    }

    void FindRobots(){
        int count = 0;
        while(transform.childCount > count){
            robots.Add(transform.GetChild(count).gameObject);
            robots[count].GetComponent<RobotBehaviour>().freeTime = roundTime;
            count++;
        }
    }

    public void PauseGame(){
        //put all robots into pausestate
        foreach (GameObject r in robots){
            RobotBehaviour rb = r.GetComponent<RobotBehaviour>();
            rb.CurrentState.EnterPauseState();
            rb.freeTime = roundTime;
        }

        for (int i = 0; i < robots.Count; i++){
            robotsMovingPreviews.Add(new GameObject());
            robotsMovingPreviews[i].name = "Robot moving trail";
            robotsMovingPreviews[i].SetActive(false);
            robotsPreview[i].Add(robots[i]);
        }
    }

    public void UnpauseGame(){
        //put all robots into play
        int index = 0;
        foreach (GameObject r in robots){
            if (r == null){
                Debug.Log("Null at " + index);
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
            robotsPreview[i].Clear();
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

    void SelectRobot(GameObject r){
        if (Input.GetMouseButtonDown(0)){
            if (selectedRobot != r){
                StopCoroutine(SetAndVisualizeTimeInput());
                for (int i = 0; i < robots.Count; i++){
                    if (r == robots[i]){
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
                if (selectedRobot == null){
                    Debug.Log("The selected robot is not know to the the TurnHandler, so therefore no commands can be given to it.");
                }
                StartCoroutine(SetAndVisualizeTimeInput());
            }
            else {
                Debug.Log("Robot already selected!");
            }
        }
    }

    IEnumerator SetAndVisualizeTimeInput(){

        if (cursorText == null){
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
        while (selectedRobot != null) {
            cursorPosition = Input.mousePosition;
            cursorScreenPosition = Camera.main.ScreenToWorldPoint(cursorPosition);

            deltaPosition = cursorScreenPosition - robotsPreview[selectedRobotIndex].Last().transform.position;

            deltaDistance = Mathf.Sqrt(Mathf.Pow(deltaPosition.x, 2) + Mathf.Pow(deltaPosition.y, 2));

            previewInputTime = secondsPerDistance * deltaDistance;
            remainingTimeForRobot = selectRB.freeTime - previewInputTime;

            if (selectedCommand == Command.AvailableCommands.Push){
                maxInputTime = selectRB.freeTime - shockwaveLife;
            }
            else {
                maxInputTime = selectRB.freeTime;
            }
            maxDeltaDistance = maxInputTime / secondsPerDistance;

            if (previewInputTime <= maxInputTime){
                timeInput = previewInputTime;
                cursorText.text = timeInput.ToString();
                cursorText.transform.position = cursorPosition;
            }
            else {
                Vector3 normalizedCursorScreenPos = cursorScreenPosition.normalized;
                Vector3 maxPosition = robotsPreview[selectedRobotIndex].Last().transform.position + new Vector3(normalizedCursorScreenPos.x * maxDeltaDistance, normalizedCursorScreenPos.y * maxDeltaDistance);
                maxPosition = Camera.main.WorldToScreenPoint(maxPosition);
                cursorText.text = timeInput.ToString();
                cursorText.transform.position = maxPosition;
            }
            yield return new WaitForSeconds(0.0001f);
        }
        cursorText.text = "";
        yield return new WaitForSeconds(0.0001f);
    }

    void GiveCommandToSelectedRobot(){
        if (selectedRobot != null && !mouseButtonIsPressed){
            //take the time for the command from the timetext
            RobotBehaviour rb = selectedRobot.GetComponent<RobotBehaviour>();
            if (timeInput > 0 && timeInput <= rb.freeTime){
                Vector3 cursorPosition = Input.mousePosition;
                Vector3 cursorScreenPosition = Camera.main.ScreenToWorldPoint(cursorPosition);

                Command command = null, previewCommand = null;
                if (selectedCommand == Command.AvailableCommands.Move){
                    previewCommand = new MoveCommand(robotsPreview[selectedRobotIndex].Last(), cursorScreenPosition, timeInput, Turns);
                    command = new MoveCommand(selectedRobot, previewCommand as MoveCommand);
                    Debug.Log("MoveCommand Added!");
                }
                else if (selectedCommand == Command.AvailableCommands.Push && timeInput <= rb.freeTime - shockWavePrefab.GetComponent<ShockwaveBehaviour>().intendedLifetime){
                    command = new PushCommand(selectedRobot, cursorScreenPosition, timeInput, Turns);
                    previewCommand = new PushCommand(robotsPreview[selectedRobotIndex].Last(), cursorScreenPosition, timeInput, Turns);
                    Debug.Log("PushCommand Added!");
                }
				else {
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

    void Update(){
        ReactToUserInput();
    }

    void ReactToUserInput(){
        if (Input.GetMouseButton(1)){
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

    public void Activate(bool active){
        activated = active;
        if (active == true) { //visually indicate that this turnhandlers robots are now active
            //start taking events
            RobotBehaviour.OnClick += new RobotBehaviour.ClickedOnRobot(SelectRobot);
            //PushCommand.OnInstantiateShockWave += new PushCommand.InstantiateShockWave(InstantiateShockwave);

            foreach (GameObject r in robots){
                r.GetComponent<RobotBehaviour>().shouldSendEvent = true;
            }
            enabled = true;
        }
        else {
            selectedRobot = null;
            timeInput = 0;
            RobotBehaviour.OnClick -= new RobotBehaviour.ClickedOnRobot(SelectRobot);

            for (int i = 0; i < robots.Count; i++) {
                robots[i].GetComponent<RobotBehaviour>().shouldSendEvent = false;
            }

            for(int i = 0; i < robotsMovingPreviews.Count; i++){
                robotsMovingPreviews[i].SetActive(false);
            }
            enabled = false;
        }
    }

    public void ReplayLastTurn(){
        //save commando lists in robots where they are longer than 0
        //and put them in that robots oldCommands<List>
        foreach (GameObject r in robots) {
            if (r.GetComponent<RobotBehaviour>().Commands.Count > 0){
                r.GetComponent<RobotBehaviour>().oldCommands = r.
                    GetComponent<RobotBehaviour>().Commands;
            }
        }
        //go through the last 8 moves and move each robot to their old position
		for (int i = moves.Count - 1; i > moves.Count - robots.Count - 1; i--){

            Move m = moves[i];
            GameObject r = m.Robot;
            r.GetComponent<Rigidbody2D>().angularVelocity = m.AngularVelocity;
            r.GetComponent<Rigidbody2D>().velocity = m.Velocity;
            r.transform.position = m.position;
            r.transform.rotation = m.rotation;

            r.GetComponent<RobotBehaviour>().Commands.AddRange(m.commands);
        }
    }

    public void RevertToOldCommands(){
        foreach (GameObject r in robots){
            if (r.GetComponent<RobotBehaviour>().oldCommands.Count > 0){
                RobotBehaviour robot = r.GetComponent<RobotBehaviour>();
                robot.commands = robot.oldCommands;
                robot.oldCommands.Clear();
            }
        }
    }
}
