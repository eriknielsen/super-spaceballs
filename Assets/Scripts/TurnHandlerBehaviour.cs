using UnityEngine;
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

    List<IEntity> entities;
    List<GameObject> robots;
    int turns;
    bool mouseButtonIsPressed = false;
    BoxCollider2D bc2D;
    float timeInput;
    List<GameObject> robotsMovingTrailHolder;

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
        bc2D = GetComponent<BoxCollider2D>();

        selectedCommand = AvailableCommands.MoveCommand;
        moves = new List<Move>();
        robots = new List<GameObject>();
        entities = new List<IEntity>();
        CreateRobots(numberOfRobots);
        robotsMovingTrailHolder = new List<GameObject>();
        for (int i = 0; i < robots.Count; i++)
        {
            robotsMovingTrailHolder.Add(new GameObject());
            robotsMovingTrailHolder[i].name = "Robot moving trail";
            robotsMovingTrailHolder[i].SetActive(false);
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
        /*
        if (robotPrefab != null)
        {
            for (int i = 0; i < numberOfRobots; i++)
            {
                float x = Random.Range(bc2D.bounds.min.x, bc2D.bounds.max.x);
                float y = Random.Range(bc2D.bounds.min.y, bc2D.bounds.max.y);
                float z = -1.0f;
                GameObject r = Instantiate(robotPrefab.gameObject, new Vector3(x, y, z), new Quaternion()) as GameObject;
                r.GetComponent<RobotBehaviour>().freeTime = roundTime;
                if (robots == null)
                {
                    Debug.Log("Null as fuuuuck");
                }
                robots.Add(r);
            }

        }
        bc2D.enabled = false;
        Component.Destroy(bc2D);
        */
        int count = 0;
        while(transform.childCount > count)
        {
            Debug.Log("hej");
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

        for (int i = 0; i < robots.Count; i++)
        {
            robotsMovingTrailHolder.Add(new GameObject());
            robotsMovingTrailHolder[i].name = "Robot moving trail";
            robotsMovingTrailHolder[i].SetActive(false);
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

        for (int i = 0; i < robotsMovingTrailHolder.Count;)
        {
            Destroy(robotsMovingTrailHolder.Last());
            robotsMovingTrailHolder.Remove(robotsMovingTrailHolder.Last());
        }
        Debug.Log("LIST LENGTH: " + robotsMovingTrailHolder.Count);
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
                StopCoroutine(SetAndDisplayTimeInput());
                for (int i = 0; i < robots.Count; i++)
                {
                    if (r == robots[i])
                    {
                        robotsMovingTrailHolder[selectedRobotIndex].SetActive(false);
                        selectedRobot = r;
                        selectedRobotIndex = i;
                        robotsMovingTrailHolder[selectedRobotIndex].SetActive(true);
                        Debug.Log("INDEX: " + i);
                        StartCoroutine(SetAndDisplayTimeInput());
                        Debug.Log("Robot selected!");
                        break;
                    }
                }
                if (selectedRobot == null)
                {
                    Debug.Log("The selected robot is not know to the the TurnHandler, so therefore no commands can be given to it.");
                }
                StartCoroutine(SetAndDisplayTimeInput());

            }
            else
            {
                Debug.Log("Robot already selected!");
            }
        }
    }

    IEnumerator SetAndDisplayTimeInput()
    {
        
        if (cursorText == null)
        {
            //om den inte hittar, instansera istället!
            cursorText = GameObject.Find("Cursor Text").GetComponent<Text>();
        }
        float secondsPerDistance = 0.3f;
        RobotBehaviour selectRB = selectedRobot.GetComponent<RobotBehaviour>();
        Vector3 cursorPosition;
        Vector3 cursorScreenPosition;
        Vector3 deltaPosition;
        float distanceFromMouse;
        float remainingTimeForRobot;
        float previewInputTime;
        float shockwaveLife = shockWavePrefab.GetComponent<ShockwaveBehaviour>().intendedLifetime;
        while (selectedRobot != null)
        {
            cursorPosition = Input.mousePosition;
            cursorScreenPosition = Camera.main.ScreenToWorldPoint(cursorPosition);

            //if there is at least one command in the robots command list, change the calculations from that command's position
            if (selectRB.commands.Count > 0)
            {
                deltaPosition = cursorScreenPosition - (Vector3)selectRB.commands[selectRB.commands.Count - 1].targetPosition;
            }
            //if the robot still doenst have any previous commands, use its position
            else
            {
                deltaPosition = cursorScreenPosition - selectedRobot.transform.position;
            }

            distanceFromMouse = Mathf.Sqrt(Mathf.Pow(deltaPosition.x, 2) + Mathf.Pow(deltaPosition.y, 2));

            previewInputTime = secondsPerDistance * distanceFromMouse;
            remainingTimeForRobot = selectRB.freeTime - previewInputTime;

            if (selectedCommand == AvailableCommands.PushCommand && previewInputTime <= selectRB.freeTime - shockwaveLife)
            {
                timeInput = previewInputTime;
                cursorText.text = timeInput.ToString();
                cursorText.transform.position = cursorPosition;
            }
            else if (selectedCommand != AvailableCommands.PushCommand && previewInputTime <= selectRB.freeTime)
            {
                Debug.Log("hejsan");
                timeInput = previewInputTime;
                cursorText.text = timeInput.ToString();
                cursorText.transform.position = cursorPosition;
            }

            Debug.Log(remainingTimeForRobot);
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
                if (selectedCommand == AvailableCommands.MoveCommand)
                {
                    MoveCommand moveCommand = new MoveCommand(selectedRobot, cursorScreenPosition, timeInput, Turns);
                    rb.Commands.Add(moveCommand);
                    Debug.Log("MoveCommand Added!");
                    MovingTrail trail;
                    //if (turns > 1)
                    //{
                    //    trail = new MovingTrail(moveCommand, timeInput, moves[selectedRobotIndex].Velocity);
                    //}
                    //else
                    //{
                    //    trail = new MovingTrail(moveCommand, timeInput, Vector2.zero);
                    //}
                    trail = new MovingTrail(moveCommand, timeInput, Vector2.zero);
                    trail.TrailGameObject.transform.parent = robotsMovingTrailHolder[selectedRobotIndex].transform;
                }
                if (selectedCommand == AvailableCommands.PushCommand && timeInput <= rb.freeTime - shockWavePrefab.GetComponent<ShockwaveBehaviour>().intendedLifetime)
                {
                    rb.Commands.Add(new PushCommand(selectedRobot, cursorScreenPosition, timeInput, 0));
                    Debug.Log("PushCommand Added!");
                }
                else
                {
                    //timeInput = 0;
                    Debug.Log("cant add push command with such a charge time");
                    return;
                }
                rb.freeTime -= timeInput;
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

    public void Activate(bool activate)
    {
        activated = activate;
        if (activate == true)
        {
            //visually indicate that this turnhandlers robots are now active

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

            for(int i = 0; i < robotsMovingTrailHolder.Count; i++)
            {
                robotsMovingTrailHolder[i].SetActive(false);
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

