using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkPlayBehaviour : NetworkBehaviour, IPlayBehaviour {

    //stuff that needs to be enabled
    public TurnHandlerBehaviour rightTurnhandlerInScene;
    public TurnHandlerBehaviour leftTurnhandlerInScene;
    public GameObject playingField;
    public GameObject robotDeselectionCollider;
    public GameObject ingameCanvas;
    public GameObject matchmakingCanvas;
    //only ever activate the player turnhandler
    TurnHandlerBehaviour playerTurnhandler;
    //recive commands from the server/client and give to this turnhandler
    TurnHandlerBehaviour otherTurnhandler;

    bool remoteIsReady = false;
    bool localIsReady = false;

    public int matchTime;
    public int roundTime;
    public int planTime;

    public bool customIsServer;
    [HideInInspector]
    public ServerBehaviour server;

    
    //number of rounds played
    int roundCount = 0;
    Coroutine gameTimerCoroutine;
    Coroutine planCountDownCoroutine;
    Goal leftGoal;
    Goal rightGoal;
    [SerializeField]
    Ball ball;
    GameTimer gameTimer;
    Text gameTimeText;
    Text planTimeText;
    bool paused = true;
    
    void Start()
    {
        
        if (customIsServer)
        {
            playerTurnhandler = rightTurnhandlerInScene;
            otherTurnhandler = leftTurnhandlerInScene;
            
        }
        else
        {
            playerTurnhandler = leftTurnhandlerInScene;
            otherTurnhandler = rightTurnhandlerInScene;
        }
        matchmakingCanvas.SetActive(false);
        ingameCanvas.SetActive(true);
        playerTurnhandler.gameObject.SetActive(true);
        playerTurnhandler.currentPlanTimeLeft = planTime;
        otherTurnhandler.gameObject.SetActive(true);
        playingField.SetActive(true);
        robotDeselectionCollider.SetActive(true);
        
        ball.gameObject.SetActive(true);
        InititializeGame();
        //activate the playeturnhandler only
        playerTurnhandler.Activate(true);
        
        planCountDownCoroutine = StartCoroutine(CountDownPlanningTime());
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
     
        gameTimer = new GameTimer(matchTime);
        if (gameTimeText == null)
        {
            gameTimeText = GameObject.Find("GameTimeText").GetComponent<Text>();
        }
        gameTimeText.text = "Time " + gameTimer.MinutesRemaining() + ":" + gameTimer.SecondsRemaining();
        if(planTimeText == null){
            planTimeText = GameObject.Find("PlanTimeText").GetComponent<Text>();
        }
        planTimeText.text = "Plan time: " + (int)playerTurnhandler.currentPlanTimeLeft;


    }
    /// <summary>
    /// assuming the same thing happens on both clients nothing needs to be sent
    /// over the network
    /// </summary>
    void OnScore()
    {
        //pause game
        if(gameTimerCoroutine != null)
            StopCoroutine(gameTimerCoroutine);

        PauseGame();
    }

    IEnumerator CountDownPlanningTime(){
        while(playerTurnhandler.currentPlanTimeLeft > 0 ){
            yield return new WaitForSecondsRealtime(1f);
            playerTurnhandler.currentPlanTimeLeft--;
        }
    }
    // Update is called once per frame
    void Update()
    {
          gameTimeText.text = "Time " + gameTimer.MinutesRemaining() + ":" + gameTimer.SecondsRemaining();
          planTimeText.text = "Plan time: " + (int)playerTurnhandler.currentPlanTimeLeft;
        if(paused){
            //if time is out then
            if(playerTurnhandler.currentPlanTimeLeft <= 0){
                    SendCommands();
                    localIsReady = true;
                    //if we are the server, tell the other client to start as well since the time has run out
                    if(customIsServer){
                        server.SendUnpauseGame();
                        StartCoroutine(UnpauseGame());
                        paused = true;
                    }
            }
            if (Input.GetKeyDown(KeyCode.Return) && paused == true && localIsReady == false)
            {
                SendCommands();
                localIsReady = true;
            }
            //if we are server, tell the other client to unpause 
            // as well as unpause the server
            if (remoteIsReady && localIsReady && customIsServer)
            {
                server.SendUnpauseGame();
                StartCoroutine(UnpauseGame());
            }
        }
    }
    IEnumerator UnpauseGame()
    {
        Debug.Log("unpausing!");
        StopCoroutine(CountDownPlanningTime());
        paused = false;
      
        playerTurnhandler.UnpauseGame();
        otherTurnhandler.UnpauseGame();
        playerTurnhandler.currentPlanTimeLeft = planTime;
        playerTurnhandler.Activate(false);
        gameTimerCoroutine = StartCoroutine(gameTimer.CountDownSeconds((int)roundTime));
       
        yield return new WaitForSeconds(roundTime);
        localIsReady = false;
        remoteIsReady = false;
        PauseGame();
    }
    void PauseGame()
    {
        playerTurnhandler.Activate(true);
        planCountDownCoroutine = StartCoroutine(CountDownPlanningTime());
        paused = true;
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
        playerTurnhandler.THDeselectRobot();
    }
    public void SelectCommand(Command.AvailableCommands c) {
        playerTurnhandler.THSelectCommand(c);
    }
}