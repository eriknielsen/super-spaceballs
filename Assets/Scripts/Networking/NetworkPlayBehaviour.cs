using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class NetworkPlayBehaviour : NetworkBehaviour, IPlayBehaviour {

    //stuff that needs to be enabled
    public TurnHandlerBehaviour rightTurnhandlerInScene;
    public TurnHandlerBehaviour leftTurnhandlerInScene;
    public GameObject playingField;
    public GameObject robotDeselectionCollider;
    public GameObject ingameCanvas;
    public GameObject matchmakingCanvas;
    public GameObject inGameMenuHandler;
    //only ever activate the player turnhandler
    TurnHandlerBehaviour playerTurnhandler;
    //recive commands from the server/client and give to this turnhandler
    TurnHandlerBehaviour otherTurnhandler;

    bool remoteIsReady = false;
    bool localIsReady = false;

    public int matchTime;
    public int roundTime;
    public int planTime;
    public int overTime;
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

    //the color when the player is actually planning
    Color activePlanTimeColor;
    Color otherTeamPlanColor;
    
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
        inGameMenuHandler.SetActive(true);
        ingameCanvas.SetActive(true);
        matchmakingCanvas.SetActive(false);
      
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
        if(playerTurnhandler == rightTurnhandlerInScene){
            activePlanTimeColor = rightGoal.scoreText.color;
            otherTeamPlanColor = leftGoal.scoreText.color;
        }
        else{
            activePlanTimeColor = leftGoal.scoreText.color;
            otherTeamPlanColor = rightGoal.scoreText.color;
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
        planTimeText.text = "" + (int)playerTurnhandler.currentPlanTimeLeft;
        planTimeText.color = activePlanTimeColor;

    }
    
    void Update()
    {
        if(gameTimer.IsGameOver()){
            StartCoroutine(HandleMatchEnd());
        }
         UpdateTimerTexts();

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
                //change color of the plantime to show that we are waiting for the other player
                planTimeText.color = otherTeamPlanColor;
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
      void UpdateTimerTexts()
    {
        string zeroBeforeMin;
		string zeroBeforeSec;
		if (gameTimer.MinutesRemaining () < 10)
			zeroBeforeMin = "0";
		else
			zeroBeforeMin = "";
		if (gameTimer.SecondsRemaining () < 10)
			zeroBeforeSec = "0";
		else
			zeroBeforeSec = "";
        if(gameTimeText != null)
        {
            gameTimeText.text = zeroBeforeMin + gameTimer.MinutesRemaining() + ":" + zeroBeforeSec + gameTimer.SecondsRemaining();
        }
     

        if(planTimeText != null && paused == true)
        {
           planTimeText.text = "" + (int)playerTurnhandler.currentPlanTimeLeft;
        }

    }
    IEnumerator HandleMatchEnd()
    {
        //check if the score is tied, then add overtime (if not already overtime) and continue
        if (leftGoal.score == rightGoal.score && gameTimer.InOvertime() == false && overTime > 0)
        {
            Debug.Log("show that overtime is happening!!");
            
            gameTimer.AddOvertime(overTime);
            
        }
        //if possible, display winner!
        else
        {
            PauseGame();
            //left won!
            if (leftGoal.score > rightGoal.score)
            {
               if(customIsServer){
                    Debug.Log("local player won!!");
                }
                else{
                    Debug.Log("local player lost!");
                }
            }
            //right won! 
            else if (rightGoal.score > leftGoal.score)
            {
                if(customIsServer){
                    Debug.Log("local player won!!");
                }
                else{
                    Debug.Log("local player lost!");
                } 
            }
            else if(rightGoal.score == leftGoal.score)
            {
                
                Debug.Log("match was even!");
            }
            //wait a bit and then change scene to mainmenu
            yield return new WaitForSeconds(1f);
            StopAllCoroutines();
            SceneManager.LoadScene("MainMenu");
        }
    }
    
    IEnumerator UnpauseGame()
    {
        if(paused == false){
            Debug.Log("breaking from unpause");
            yield break;
        }
        Debug.Log("unpausing");
        StopCoroutine(planCountDownCoroutine);
        paused = false;
        ball.Unpause();
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
     
        if(paused== true){
            return;
        }
        playerTurnhandler.Activate(true);
        planTimeText.color = activePlanTimeColor;
        planCountDownCoroutine = StartCoroutine(CountDownPlanningTime());
        paused = true;
        playerTurnhandler.PauseGame();
        otherTurnhandler.PauseGame();
        if(customIsServer){
            List<GameObject> allRobots = new List<GameObject>();
            allRobots.AddRange(playerTurnhandler.Robots);
            allRobots.AddRange(otherTurnhandler.Robots);
            server.SendSyncStateMsg(allRobots);
            Debug.Log("sending sycnstate msg");
        }

        ball.Pause();
    }
    /// <summary>
    /// turns serializablecommands to real commmands
    /// </summary>
    /// <param name="serializableCommands"></param>
    void PutCommandsIntoRobots(List<SerializableCommand> serializableCommands)
    {
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
        
        Dictionary<int, List<Command>> commandDict = new Dictionary<int, List<Command>>();
        commandDict = GetCommandDict(playerTurnhandler.Robots);
        
        for(int i = 0; i < playerTurnhandler.Robots.Count;i++){
            if(playerTurnhandler.Robots[i].GetComponent<RobotBehaviour>().Commands.Count > 0){
  //Debug.Log(playerTurnhandler.Robots[i].GetComponent<RobotBehaviour>().Commands[0].targetPosition.x + " y: " +  //playerTurnhandler.Robots[i].GetComponent<RobotBehaviour>().Commands[0].targetPosition.y);
            }
          
        }
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

        //Debug.Log(scList.Count + " commands added to the list, asking serverbheaviour to send them!");
        server.SendCommands(scList);
    }

    //Message callbacks
    public void OnRecieveUnpause(NetworkMessage netMsg)
    {
        Debug.Log("unpause msg rec!");
        if(paused == true)
            StartCoroutine(UnpauseGame());
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
        //Debug.Log(deserializedCommands.Count + " commands recived!");
        PutCommandsIntoRobots(deserializedCommands);

        remoteIsReady = true;
    }
    public void OnRecieveSyncState(NetworkMessage netMsg){
        if(customIsServer == false){
            ServerBehaviour.SyncStateMsg msg = netMsg.ReadMessage<ServerBehaviour.SyncStateMsg>();
            ServerBehaviour.SerializablePositionList deserializedPositions = new ServerBehaviour.SerializablePositionList();

            ServerBehaviour.SerializablePositionList deserializedVelocities = new ServerBehaviour.SerializablePositionList();

            BinaryFormatter bf = new BinaryFormatter();
            Byte[] buffer = msg.robotPositions;
           
            System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer);
            deserializedPositions = bf.Deserialize(ms) as ServerBehaviour.SerializablePositionList;

            Debug.Log(deserializedPositions.Count +  " positions recived!");


            BinaryFormatter bf2 = new BinaryFormatter();
            Byte[] buffer2 = msg.robotVelocities;
            Debug.Log(buffer2.Length);
            System.IO.MemoryStream ms2 = new System.IO.MemoryStream(buffer2);
            
            deserializedVelocities = bf2.Deserialize(ms2) as ServerBehaviour.SerializablePositionList;
            Debug.Log(deserializedVelocities.Count + " velocities recived!");
            //put the positons into the turnhandlers robots
            //the first robotCount(3) robots should be put in the otherTurnhandler
            
            for(int i = 0; i < 3;i++){
                GameObject r = otherTurnhandler.Robots[i];
                //positions
                r.transform.position = 
                deserializedPositions[i].V2();
                //and velocities

                //prevVelocity is supposed to be changed now AFTER the game has been paused
                r.GetComponent<RobotBehaviour>().prevVelocity = deserializedVelocities[i].V2();


            }
            //and the remaining 3 should be put in the playerTurnhandler
            for(int i = 3; i < 6;i++){
                 GameObject r = playerTurnhandler.Robots[i-3];
                r.transform.position =
                deserializedPositions[i].V2();

                //prevVelocity is supposed to be changed now AFTER the game has been paused
                r.GetComponent<RobotBehaviour>().prevVelocity = deserializedVelocities[i].V2();
                
            }   
            Debug.Log(deserializedPositions[0].x + " y: " + deserializedPositions[0].y);
        }
        
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
                //for(int j = 0; j < rb.Commands.Count;j++ ){
                    //Debug.Log("x: " + rb.Commands[j].targetPosition.x + " y:" + rb.Commands[j].targetPosition.y);
                //}
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