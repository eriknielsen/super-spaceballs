using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkPlayBehaviour : NetworkBehaviour, IPlayBehaviour {

    public GameObject rightTurnhandlerPrefab;
    public GameObject leftTurnhandlerPrefab;
    //only ever activate the player turnhandler
    TurnHandlerBehaviour playerTurnhandler;
    //recive commands from the server/client and give to this turnhandler
    TurnHandlerBehaviour otherTurnhandler;

    bool remoteIsReady = false;
    bool localIsReady = false;

    public int matchTime;
    public int roundTime;

    public bool customIsServer;
    public ServerBehaviour server;

    
    //number of rounds played
    int roundCount = 0;

    Goal leftGoal;
    Goal rightGoal;
    Ball ball;
    GameTimer gameTimer;
    Text gameTimeText;
    // Use this for initialization
    void Start()
    {
        InititializeGame();
        if (customIsServer)
        {
            playerTurnhandler = Instantiate(rightTurnhandlerPrefab).GetComponent<TurnHandlerBehaviour>();
            otherTurnhandler = Instantiate(leftTurnhandlerPrefab).GetComponent<TurnHandlerBehaviour>();
        }
        else
        {
            playerTurnhandler = Instantiate(leftTurnhandlerPrefab).GetComponent<TurnHandlerBehaviour>();
            otherTurnhandler = Instantiate(rightTurnhandlerPrefab).GetComponent<TurnHandlerBehaviour>();

        }

        //activate the playeturnhandler only
        playerTurnhandler.Activate(true);

    }
    void InititializeGame()
    {
        leftGoal = GameObject.Find("LeftGoal").GetComponent<Goal>();
        rightGoal = GameObject.Find("RightGoal").GetComponent<Goal>();
        //event callbacks for scoring
        if (leftGoal != null || rightGoal != null)
        {
            Goal.OnGoalScored += new Goal.GoalScored(OnScore);
        }
        else {
            Debug.Log("couldint find goals :(");
        }
        ball = GameObject.Find("Ball").GetComponent<Ball>();
        gameTimer = new GameTimer(matchTime);
        if (gameTimeText == null)
        {
            gameTimeText = GameObject.Find("GameTimeText").GetComponent<Text>();
        }
        gameTimeText.text = "Time " + gameTimer.MinutesRemaining() + ":" + gameTimer.SecondsRemaining();

    }
    /// <summary>
    /// assuming the same thing happens on both clients nothing needs to be sent
    /// over the network
    /// </summary>
    void OnScore()
    {
        //pause game
        StopAllCoroutines();
        PauseGame();
       

    }


    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendCommands();
            localIsReady = true;
        }
        if (remoteIsReady && localIsReady && customIsServer)
        {
            server.SendUnpauseGame();
            StartCoroutine(UnpauseGame(false));
        }
    }
    IEnumerator UnpauseGame(bool asReplay)
    {
        localIsReady = false;
        remoteIsReady = false;
        playerTurnhandler.UnpauseGame();
        otherTurnhandler.UnpauseGame();
        yield return new WaitForSeconds(roundTime);
        PauseGame();
    }
    void PauseGame()
    {
        playerTurnhandler.PauseGame();
        otherTurnhandler.PauseGame();
        ball.Pause();
    }
    /// <summary>
    /// turns serializablecommands to real commmands
    /// </summary>
    /// <param name="serializableCommands"></param>
    void PutCommandsIntoRobots(List<SerializableCommand> serializableCommands)
    {
        Debug.Log("turning " + serializableCommands.Count + " commands into real commands");
        foreach (SerializableCommand sc in serializableCommands)
        {

            switch (sc.type)
            {
                case SerializableCommand.CommandType.Move:
                    otherTurnhandler.Robots[sc.robotIndex].GetComponent<RobotBehaviour>().Commands.Add(
                        new MoveCommand(otherTurnhandler.Robots[sc.robotIndex], sc.targetPosition.V2(), sc.lifeDuration, 0));

                    break;
                case SerializableCommand.CommandType.Push:
                    otherTurnhandler.Robots[sc.robotIndex].GetComponent<RobotBehaviour>().Commands.Add(
                        new PushCommand(otherTurnhandler.Robots[sc.robotIndex], sc.targetPosition.V2(), sc.lifeDuration, 0));
                    break;
                default:
                    Debug.Log("no case for that commandtype: " + sc.type);
                    break;
            }
        }
    }
    //#####
    // país mágico
    //#####

    /*
        write function that makes sure that things like 
        gametimer, score 
        is synced
        also handle situations where score isn't synced......
        
    */
    //send commands to the other client
    void SendCommands()
    {
        Debug.Log("try to convert and send commands");
        Dictionary<int, List<Command>> commandDict = new Dictionary<int, List<Command>>();
        commandDict = GetCommandDict(playerTurnhandler.Robots);
        ServerBehaviour.SerializableCommandList scList = new ServerBehaviour.SerializableCommandList();


        foreach (KeyValuePair<int, List<Command>> pair in commandDict)
        {
            for (int i = 0; i < pair.Value.Count; i++)
            {
                Command c = pair.Value[i];
                Type t = c.GetType();
                if (t == typeof(MoveCommand))
                {
                    SerializableCommand sc = new SerializableCommand(pair.Key, c.targetPosition, c.lifeDuration, SerializableCommand.CommandType.Move, 0);
                    scList.Add(sc);

                }
                else if (t == typeof(PushCommand))
                {
                    SerializableCommand sc = new SerializableCommand(pair.Key, c.targetPosition, c.lifeDuration, SerializableCommand.CommandType.Push, 0);
                    scList.Add(sc);
                }

            }
        }

        Debug.Log(scList.Count + " commands added to the list, asking serverbheaviour to send them!");
        server.SendCommands(scList);
    }

    //Message callbacks
    public void OnRecieveUnpause(NetworkMessage netMsg)
    {
        Debug.Log("unpause msg rec!");
        StartCoroutine(UnpauseGame(false));
    }
    /// <summary>
    /// Recieve and put the commands into the otherTurnhandler's robots
    /// Also means that the other player is ready
    /// </summary>
    /// <param name="netMsg"></param>
    public void OnRecieveCommands(NetworkMessage netMsg)
    {
        List<SerializableCommand> deserializedCommands = new List<SerializableCommand>();
        BinaryFormatter bf = new BinaryFormatter();
        Byte[] buffer = netMsg.ReadMessage<ServerBehaviour.CommandMsg>().serializedCommands;
        System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer);
        deserializedCommands = bf.Deserialize(ms) as List<SerializableCommand>;
        Debug.Log(deserializedCommands.Count + " commands recived!");
        PutCommandsIntoRobots(deserializedCommands);

        remoteIsReady = true;
    }
    //####
    // Utility functions
    //####
    Dictionary<int, List<Command>> GetCommandDict(List<GameObject> robotList)
    {
        Dictionary<int, List<Command>> dict = new Dictionary<int, List<Command>>();
        for (int i = 0; i < robotList.Count; i++)
        {

            RobotBehaviour rb = robotList[i].GetComponent<RobotBehaviour>();
            if (rb.Commands.Count > 0)
            {
                dict.Add(i, rb.Commands);
            }
        }
        return dict;
    }

    public void DeselectRobot()
    {

    }
    public void SelectCommand(Command.AvailableCommands c) {

    }
}