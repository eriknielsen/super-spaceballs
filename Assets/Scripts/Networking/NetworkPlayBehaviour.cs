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
	public bool commandsSent = false;
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
    
    void Start(){
        if (customIsServer){
            playerTurnhandler = rightTurnhandlerInScene;
            otherTurnhandler = leftTurnhandlerInScene;   
        }
        else {
            playerTurnhandler = leftTurnhandlerInScene;
            otherTurnhandler = rightTurnhandlerInScene;
        }
		matchmakingCanvas.SetActive(false);

        ingameCanvas.SetActive(true);
        playingField.SetActive(true);
		ball.gameObject.SetActive(true);
		robotDeselectionCollider.SetActive(true);
		otherTurnhandler.gameObject.SetActive(true);
		playerTurnhandler.gameObject.SetActive(true);
		playerTurnhandler.currentPlanTimeLeft = planTime;

        InititializeGame();
        //activate the playeturnhandler only
        playerTurnhandler.Activate(true);
        otherTurnhandler.Activate(false);
        planCountDownCoroutine = StartCoroutine(CountDownPlanningTime());
    }

    void InititializeGame(){
        leftGoal = GameObject.Find("LeftGoal").GetComponent<Goal>();
        rightGoal = GameObject.Find("RightGoal").GetComponent<Goal>();
        //event callbacks for scoring
        if (leftGoal != null && rightGoal != null){
            Goal.OnGoalScored += new Goal.GoalScored(OnScore);
        }
        else {
            Debug.Log("couldint find goals :(");
        }
        if (playerTurnhandler == rightTurnhandlerInScene){
			activePlanTimeColor = ToolBox.Instance.rightTeamColor;
			otherTeamPlanColor = ToolBox.Instance.leftTeamColor;
        }
        else {
            Debug.Log(leftGoal+ " rightGoal " + rightGoal);
            activePlanTimeColor = ToolBox.Instance.leftTeamColor;
			otherTeamPlanColor = ToolBox.Instance.rightTeamColor;
        }
        
        gameTimer = new GameTimer(matchTime);
        if (gameTimeText == null){
            gameTimeText = GameObject.Find("GameTimeText").GetComponent<Text>();
        }
        gameTimeText.text = "Time " + gameTimer.MinutesRemaining() + ":" + gameTimer.SecondsRemaining();
        if(planTimeText == null){
            planTimeText = GameObject.Find("PlanTimeText").GetComponent<Text>();
        }
        planTimeText.text = "" + (int)playerTurnhandler.currentPlanTimeLeft;
        planTimeText.color = activePlanTimeColor;
    }

    void Update(){
        if(gameTimer.IsGameOver()){
            StartCoroutine(HandleMatchEnd());
        }
         UpdateTimerTexts();

        if(paused){
            //if time is out then
            if(playerTurnhandler.currentPlanTimeLeft < 1){
                Debug.Log("timeout!!");
                //only do this part once since we will go in here again
                    //while waiting for server.recivedCommands to be set to true
                if(commandsSent == false){
					Debug.Log("Send those commands!");
					SendCommands();
					localIsReady = true;
				} 
				//if we are the server, tell the other client to start as well since the time has run out

				//here we have to wait first for the commands to arrive before
                    //unpausing the clients
				if(customIsServer && server.recivedCommands){
					server.recivedCommands = false;
					server.SendUnpauseGame();
					StartCoroutine(UnpauseGame());

					Debug.Log("unpausing since time ran out");
				}
            }
            else if (Input.GetKeyDown(KeyCode.Return) && paused == true && localIsReady == false){
                SendCommands();
                localIsReady = true;
                //change color of the plantime to show that we are waiting for the other player
                planTimeText.color = otherTeamPlanColor;
            }
            //if we are server, tell the other client to unpause 
            // as well as unpause the server
            else if (remoteIsReady && localIsReady && customIsServer && paused == true){
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

        //if it was because we were descyned then idk, do something!
        
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
        playerTurnhandler.Activate(false);
        Time.timeScale = 1;
        paused = false;
        StopCoroutine(planCountDownCoroutine);
       
        ball.Unpause();
        playerTurnhandler.UnpauseGame();
        otherTurnhandler.UnpauseGame();
        localIsReady = false;
        remoteIsReady = false;
       
        gameTimerCoroutine = StartCoroutine(gameTimer.CountDownSeconds((int)roundTime));
        playerTurnhandler.currentPlanTimeLeft = planTime;
        
        yield return new WaitForSeconds(roundTime);
        
      
        PauseGame();
    }
    void PauseGame()
    {
     
        if(paused== true){
            Debug.Log("game already paused, returning");
            return;
        }
        StopCoroutine(gameTimerCoroutine);
        paused = true;
        commandsSent = false;
        server.recivedCommands = false;
        playerTurnhandler.Activate(true);
        planTimeText.color = activePlanTimeColor;
        playerTurnhandler.PauseGame();
        otherTurnhandler.PauseGame();
        ball.Pause();
        planCountDownCoroutine = StartCoroutine(CountDownPlanningTime());
        
       
        if(customIsServer){
            List<GameObject> allRobots = new List<GameObject>();
            allRobots.AddRange(playerTurnhandler.Robots);
            allRobots.AddRange(otherTurnhandler.Robots);
            server.SendSyncStateMsg(allRobots, ball.gameObject, new Position(leftGoal.score,rightGoal.score));
           
        }

        
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
                        new MoveCommand(otherTurnhandler.Robots[sc.robotIndex], sc.initialForce.V2(), sc.force.V2(),sc.lifeDuration));

                    break;
                case SerializableCommand.CommandType.Push:
                    otherTurnhandler.Robots[sc.robotIndex].GetComponent<RobotBehaviour>().Commands.Add(
                        new PushCommand(otherTurnhandler.Robots[sc.robotIndex], sc.targetPosition.V2(), sc.force.V2(),sc.lifeDuration));
                    break;
                default:
                    Debug.Log("no case for that commandtype: " + sc.type);
                    break;
            }
        }
        server.recivedCommands = true;
    }
    //#####
    // país mágico
    //#####

    //send commands to the other client
    void SendCommands()
    {
        commandsSent = true;
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
                    MoveCommand mc = c as MoveCommand;
                    SerializableCommand sc = new SerializableCommand(pair.Key, c.targetPosition, c.lifeDuration, SerializableCommand.CommandType.Move, 0,
                    mc.Force, mc.InitialForce);
                  
                    scList.Add(sc);

                }
                else if (t == typeof(PushCommand))
                {
                    PushCommand pc = c as PushCommand;
                   
                    SerializableCommand sc = new SerializableCommand(pair.Key, c.targetPosition, c.lifeDuration, SerializableCommand.CommandType.Push, 0,
                    pc.Velocity,Vector2.zero);
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
        if(paused && server.recivedCommands == false){
            List<SerializableCommand> deserializedCommands = new List<SerializableCommand>();
            BinaryFormatter bf = new BinaryFormatter();
            Byte[] buffer = netMsg.ReadMessage<ServerBehaviour.CommandMsg>().serializedCommands;
            System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer);
            deserializedCommands = bf.Deserialize(ms) as List<SerializableCommand>;

            PutCommandsIntoRobots(deserializedCommands);

            remoteIsReady = true;
            
        }
        else{
            Debug.Log("game is paused: " + paused);
        }
        
        
    }
    public IEnumerator OnRecieveSyncStateCoroutine(NetworkMessage  netMsg){
        NetworkMessage message = netMsg;
        //if game is not paused, then wait untill it is and then call the "real" OnRecieveSyncState
        while(paused == false){
            yield return new WaitForFixedUpdate();
        }
        OnRecieveSyncState(message);
    }
    //ASSUMES THERE IS EXACTLY 3 ROBOTS PER TEAM + ONE BALL + a score position vector
    public void OnRecieveSyncState(NetworkMessage netMsg){
        if(!paused){
            Debug.Log("OnRecieveSyncStatecouroutiney");
            StartCoroutine(OnRecieveSyncStateCoroutine(netMsg));
            
            return;
        }
        if(customIsServer == false){
            ServerBehaviour.SyncStateMsg msg = netMsg.ReadMessage<ServerBehaviour.SyncStateMsg>();
            //the buffer msg consists of
            //3 pairs of positions and a velocity (both type of Position)
            //followed by the ball's position and velocity
            ServerBehaviour.SerializablePositionList deserializedBuffer = new ServerBehaviour.SerializablePositionList();

          
            BinaryFormatter bf = new BinaryFormatter();
            Byte[] buffer = msg.info;
           
            System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer);
            deserializedBuffer = bf.Deserialize(ms) as ServerBehaviour.SerializablePositionList;

            
            int robotIndex = 0;
            for(int i = 0; i < deserializedBuffer.Count;i++){
              
                //for the first 6 robots, put info in the otherTurnhandler
                if(i < 6){
                    GameObject r = otherTurnhandler.Robots[robotIndex];
                    //even for position
                     if(i % 2 == 0){
                         
                        r.transform.position = deserializedBuffer[i].V2();
                        //Debug.Log(r.transform.position.x);
                     }
                     //odd for velocity
                     else{
                         r.GetComponent<RobotBehaviour>().prevVelocity = deserializedBuffer[i].V2();
                         //next robot
                        robotIndex++;
                     }
                }
                //for the next 6 things, put in playerTurnhandler
                else if(i < 12){
                    GameObject r = playerTurnhandler.Robots[robotIndex-3];
                    //even for position
                    if(i % 2 == 0){
                        r.transform.position = deserializedBuffer[i].V2();
                    }
                    //odd for velocity
                    else{
                        r.GetComponent<RobotBehaviour>().prevVelocity = deserializedBuffer[i].V2();
                        //next robot
                        robotIndex++;
                    }
                }
                //the last two things are the ball and score!
                else {
                    ball.transform.position = deserializedBuffer[i].V2();
                    ball.GetComponent<Ball>().PreviousVelocity = deserializedBuffer[i+1].V2();
                    //tell ball to redraw it's linerenderthingy
                    ball.DrawTrajectory();

                    if(leftGoal.score != deserializedBuffer[i+2].x || rightGoal.score != deserializedBuffer[i+2].y){
                        leftGoal.score = (int)deserializedBuffer[i+2].x;
                        rightGoal.score = (int)deserializedBuffer[i+2].y;
                        OnScore();
                    }

                    break;
                }
               
            }
            
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