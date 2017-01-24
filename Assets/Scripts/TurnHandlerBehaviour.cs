using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnHandlerBehaviour : MonoBehaviour {

    private GameObject selectedRobot;
    private AvailableCommands selectedCommand;
    private enum AvailableCommands { MoveCommand};
    private List<RobotBehaviour> testRobots;

    public GameObject testRobot;
    public List<GameObject> robots;
    public int turns;
    //en lista med drag
    public List<Move> moves;
	// Use this for initialization
	void Start () {
        selectedCommand = AvailableCommands.MoveCommand;
        testRobots = new List<RobotBehaviour>();
        testRobots = FindObjectsOfType<RobotBehaviour>().ToList();

        moves = new List<Move>();
        RobotBehaviour.OnClick += new RobotBehaviour.ClickedOnRobot(ChooseRobot);
        CreateTestRobots();
        TestRobotCommands();
        UnpauseGame();
        //remember which commands were for which turn
        turns = 1;
        StartCoroutine(TestPausing());

    }
	void TestRobotCommands()
    { 
        List<Command> testCommands = new List<Command>();
          
        foreach (GameObject r in robots)
        {
            Debug.Log("testing the move command");
            Command moveForward = new MoveCommand(r, new Vector2(3, 3), 1,turns);
            Command moveBack = new MoveCommand(r, new Vector2(-1, 0), 1,turns);

            r.GetComponent<RobotBehaviour>().commands.Add(moveForward);
            r.GetComponent<RobotBehaviour>().commands.Add(moveBack);
        } 
        
    }
    void CreateTestRobots()
    {
        if (testRobot != null)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject r = Instantiate(testRobot, new Vector2(i + 1, i + 1), new Quaternion()) as GameObject;
                robots.Add(r);
            }
        }
    }

    public void PauseGame()
    {
        //change timescale
        Time.timeScale = 0;
        //put all robots into pausestate
        foreach(GameObject r in robots)
        {
           
            r.GetComponent<RobotBehaviour>().currentState.EnterPauseState();
        }
    }
    public void UnpauseGame()
    {
        //change timescale
        Time.timeScale = 1;
        //put all robots into play
        foreach (GameObject r in robots)
        {
            moves.Add(new Move(r, turns, r.GetComponent<RobotBehaviour>().commands));
            r.GetComponent<RobotBehaviour>().currentState.EnterPlayState();
        }
        turns++;
    }

    IEnumerator TestPausing()
    {
        yield return new WaitForSeconds(4f);
        PauseGame();
        yield return new WaitForSeconds(1f);
        Debug.Log("s");
        //UndoLastMove();

    }

    void UndoLastMove()
    {
        if (turns > 0)
        {
            //remove all shockwaves
            //reset all robots to the previous' move's position
            int turnIndex = 0;
            for (int i = turns - 1; i < turns * robots.Count; i++)
            {

                turnIndex = (turns - 1) * robots.Count + i;
                robots[i].transform.position = moves[turnIndex].position;
                robots[i].GetComponent<Rigidbody2D>().velocity = moves[turnIndex].velocity;
            }
        }

    }

    void ChooseRobot(GameObject r)
    {
        selectedRobot = r;
        Debug.Log("Robot selected!");
        Destroy(selectedRobot);
    }

    void GiveCommandToSelectedRobot()
    {
        if(selectedRobot != null)
        {
            if(selectedCommand == AvailableCommands.MoveCommand)
            {
                Vector3 mousePosition = Input.mousePosition;
                Vector3 pointPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                selectedRobot.GetComponent<RobotBehaviour>().commands.Add(new MoveCommand(selectedRobot, pointPosition, 1, turns));
                Debug.Log("Command Added!");
            }
        }
    }
    
    void Update()
    {
        DetectMouseClick();
    }

    void DetectMouseClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            GiveCommandToSelectedRobot();
        }
    }
    /*
    void UndoLastMove()
    {
        if (turns > 0)
        {
            //remove all shockwaves
            //reset all robots to the previous' move's position
            int turnIndex = 0;
            for (int i = turns - 1; i < turns * robots.Count; i++)
            {
                turnIndex = (turns - 1) * robots.Count + i;
                robots[i].transform.position = moves[turnIndex].position;
                robots[i].GetComponent<Rigidbody2D>().velocity = moves[turnIndex].velocity;
            }
        }
    }
    */
}
