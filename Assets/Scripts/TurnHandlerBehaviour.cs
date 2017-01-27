using UnityEngine;
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
        RobotBehaviour.OnClick += new RobotBehaviour.ClickedOnRobot(ChooseRobot);
        robots = new List<GameObject>();

        CreateRobots();
        PauseGame();
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
            Debug.Log("Robots in list: " + robots.Count);
        }
    }

    public void PauseGame()
    {
        //change timescale
        Time.timeScale = 0;
        //put all robots into pausestate
        foreach (GameObject r in robots)
        {
            r.GetComponent<RobotBehaviour>().CurrentState.EnterPauseState();
        }
    }
    public void UnpauseGame()
    {
        //change timescale
        Time.timeScale = 1;
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

    void ActivateRobots()
    {
        if (robots != null)
        {
            for (int i = 0; i < robots.Count; i++)
            {
                robots[i].GetComponent<RobotBehaviour>().CurrentState.EnterPlayState();
                //starta en coroutine per robot, på en och samma robot?
                StartCoroutine(RobotActivatedDuration());
                Debug.Log("Round started!");
                UnpauseGame();
            }
        }


    }
    //test function?
    IEnumerator RobotActivatedDuration()
    {
        Debug.Log("Co-routine started!");
        yield return new WaitForSeconds(roundTime);
        selectedRobot.GetComponent<RobotBehaviour>().CurrentState.EnterPauseState();
        PauseGame();
        Debug.Log("Game paused!");
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
        if (Input.GetButtonDown("StartRound"))
        {
            ActivateRobots();
        }
    }
    public void Activate(bool b)
    {
        if (b)
        {
            RobotBehaviour.OnClick += new RobotBehaviour.ClickedOnRobot(ChooseRobot);
        }
        else
        {
            RobotBehaviour.OnClick -= new RobotBehaviour.ClickedOnRobot(ChooseRobot);

        }

    }
}
