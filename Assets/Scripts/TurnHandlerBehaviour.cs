using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

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

    private Text cursorText;

    private GameObject selectedRobot;
    private AvailableCommands selectedCommand;
    private enum AvailableCommands { MoveCommand, PushCommand };

    List<IEntity> entities;
    List<GameObject> robots;
    int turns;
    bool mouseButtonIsPressed = false;
    BoxCollider2D bc2D;
    float timeInput;
  
    public float intendedShockwaveLifetime;

    //en lista med drag
    public List<Move> moves;

    public int Turns
    {
        get
        {
            return turns;
        }
    }

    public int NumberOfRobots
    {
        get
        {
            return numberOfRobots;
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
        entities = new List<IEntity>();

        CreateRobots(numberOfRobots);

        
        turns = 1;
    }

    void OnDestroy()
    {
        DestroyRobots();
    }

    void CreateRobots(int numberOfRobots)
    {
        if (robotPrefab != null)
        {
            for (int i = 0; i < numberOfRobots; i++)
            {
                float x = Random.Range(bc2D.bounds.min.x, bc2D.bounds.max.x);
                float y = Random.Range(bc2D.bounds.min.y, bc2D.bounds.max.y);
                GameObject r = Instantiate(robotPrefab.gameObject, new Vector2(x, y), new Quaternion()) as GameObject;
                r.GetComponent<RobotBehaviour>().freeTime = roundTime;
                if (robots == null)
                {
                    Debug.Log("Null as fuuuuck");
                }
                robots.Add(r);
            }

        }
        bc2D.enabled = false;
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


        //remove IEntities who's gameobjects have been removed
        //and pause the rest
        List<IEntity> newList = new List<IEntity>();
        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i] != null)
                newList.Add(entities[i]);

        }
        entities = newList;
        for (int i = 0; i < entities.Count; i++)
        {

            if (entities[i] != null)
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
        foreach (IEntity e in entities)
        {
            if (e != null)
            {
                e.EnterPlay();
            }
            else
            {
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

            StartCoroutine(MeasureAndDisplayTimeInput());
            Debug.Log("Robot selected!");
        }
    }

    IEnumerator MeasureAndDisplayTimeInput()
    {
        if (cursorText == null)
        {
            cursorText = GameObject.Find("Cursor Text").GetComponent<Text>();
        }
        float secondsPerDistance = 0.5f;
        RobotBehaviour selectRB = selectedRobot.GetComponent<RobotBehaviour>();
        Vector3 cursorPosition;
        Vector3 cursorScreenPosition;
        Vector3 deltaPosition;
        float distanceFromMouse;
        float remainingTimeForRobot;
        float previewInputTime;
        while (selectedRobot != null)
        {
            cursorPosition = Input.mousePosition;
            cursorScreenPosition = Camera.main.ScreenToWorldPoint(cursorPosition);
            //Vector3 cursorViewportPoint = Camera.main.WorldToViewportPoint(cursorPosition);
            //if there is at least one command in the robots command list, change the calculations from that command's position
            if(selectRB.commands.Count > 0)
            {
                deltaPosition = cursorScreenPosition - (Vector3)selectRB.commands[selectRB.commands.Count-1].targetPosition;
            }
            //if the robot still doenst have any previous commands, use its position
            else
            {
                deltaPosition = cursorScreenPosition - selectedRobot.transform.position;
            }
           
            distanceFromMouse = Mathf.Sqrt(Mathf.Pow(deltaPosition.x, 2) + Mathf.Pow(deltaPosition.y, 2));
            
            previewInputTime = secondsPerDistance * distanceFromMouse;
            remainingTimeForRobot = selectRB.freeTime - previewInputTime;
            
            if (selectedCommand == AvailableCommands.PushCommand && previewInputTime <= selectRB.freeTime - intendedShockwaveLifetime)
            {
                timeInput = previewInputTime;
                cursorText.text = timeInput.ToString();
                cursorText.transform.position = cursorPosition;
                
            }
            else if (previewInputTime <= selectRB.freeTime)
            {
                timeInput = previewInputTime;
                cursorText.text = timeInput.ToString();
                cursorText.transform.position = cursorPosition;
            }
            
            
            yield return new WaitForSeconds(0.0001f);
        }
        cursorText.text = "";
        yield return null;
    }

    float AngleBetweenPoints(Vector2 point1, Vector2 point2)
    {
        Vector2 delta = point1 - point2;
        float angle = Mathf.Atan2(delta.y, delta.x);
        if (angle < 0)
        {
            angle = 2 * Mathf.PI + angle;
        }
        return angle;
    }

    void GiveCommandToSelectedRobot()
    {
        if (selectedRobot != null && !mouseButtonIsPressed)
        {
            //take the time for the command from the timetext
            float duration = timeInput;
            RobotBehaviour rb = selectedRobot.GetComponent<RobotBehaviour>();
            if(timeInput <= rb.freeTime)
            {
                Vector3 cursorPosition = Input.mousePosition;
                Vector3 cursorScreenPosition = Camera.main.ScreenToWorldPoint(cursorPosition);
                if (selectedCommand == AvailableCommands.MoveCommand)
                {

                    rb.Commands.Add(new MoveCommand(selectedRobot, cursorScreenPosition, duration, Turns));
                    Debug.Log("MoveCommand Added!");
                }
                if (selectedCommand == AvailableCommands.PushCommand)
                {
                    float speed = 8.0f;

                    float angle = AngleBetweenPoints(cursorScreenPosition, selectedRobot.transform.position);
                    Vector2 velocity = new Vector2(speed * Mathf.Cos(angle), speed * Mathf.Sin(angle));
                    rb.Commands.Add(new PushCommand(selectedRobot, velocity, duration));
                    Debug.Log("PushCommand Added!");
                }
                rb.freeTime -= duration;
                timeInput = 0;
            }
           
            
        }
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
    void OnEnable()
    {
        Debug.Log("Enabled!");
    }

    void OnDisable()
    {
        Debug.Log("Disabled!");
    }
    public void Activate(bool activate)
    {
        if (activate == true)
        {
            //visually indicate that this turnhandlers robots are now active

            //start taking events
            RobotBehaviour.OnClick += new RobotBehaviour.ClickedOnRobot(ChooseRobot);
            //PushCommand.OnInstantiateShockWave += new PushCommand.InstantiateShockWave(InstantiateShockwave);

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
}
