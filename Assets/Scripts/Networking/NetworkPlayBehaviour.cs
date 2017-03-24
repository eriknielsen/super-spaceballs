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

    public GameObject AnimationObjects;

    bool remoteIsReady = false;
    bool localIsReady = false;
    bool animatingOvertime = false;

    public int matchTime;
    public int roundTime;
    public int planTime;
    public int overTime;
    public bool customIsServer;
	public bool commandsSent = false;
    [HideInInspector]
    public ServerBehaviour server;

   
	Coroutine handleMatchEnd;
    Coroutine gameTimerCoroutine;
	Coroutine countDownPlanningTime;
    Coroutine UnpauseGameCoroutine;
    Goal leftGoal;
    Goal rightGoal;
    [SerializeField]
    Ball ball;
    GameTimer gameTimer;
    Animator endOfMatchAnimator;

    Text gameTimeText;
    Text planTimeText;

    bool paused = true;

    //the color when the player is actually planning
    Color activePlanTimeColor;
    Color otherTeamPlanColor;

    Animator endOfMatchAnim, playerTurnAnim;
	OvertimeAnimScript overtimeAnimScript;
    Button endTurnButton;
    bool allowTurnEnd;
    string localPlayerSide;

    
    void Start(){
        if (customIsServer){
            playerTurnhandler = rightTurnhandlerInScene;
            otherTurnhandler = leftTurnhandlerInScene;   
            localPlayerSide = "RightTurn";
        }
        else {
            playerTurnhandler = leftTurnhandlerInScene;
            otherTurnhandler = rightTurnhandlerInScene;
            localPlayerSide = "LeftTurn";
        }
		matchmakingCanvas.SetActive(false);
   
        
        ingameCanvas.SetActive(true);
        inGameMenuHandler.SetActive(true);
        playingField.SetActive(true);
		ball.gameObject.SetActive(true);
		robotDeselectionCollider.SetActive(true);
		otherTurnhandler.gameObject.SetActive(true);
		playerTurnhandler.gameObject.SetActive(true);
		playerTurnhandler.currentPlanTimeLeft = planTime;
        AnimationObjects.SetActive(true);
        endOfMatchAnim = GameObject.Find("EndOfMatchAnimation").GetComponent<Animator>();
		playerTurnAnim = GameObject.Find("PlayerTurnAnimation").GetComponent<Animator>();
		overtimeAnimScript = GameObject.Find("OvertimeAnimation").GetComponent<OvertimeAnimScript>();
		endTurnButton = GameObject.Find("EndTurnButton").GetComponent<Button>();
        InititializeGame();
        //activate the playeturnhandler only
        playerTurnhandler.Activate(true);
        otherTurnhandler.Activate(false);
        countDownPlanningTime = StartCoroutine(CountDownPlanningTime());
        allowTurnEnd = true;
        
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
			activePlanTimeColor = ToolBox.Instance.RightTeamColor;
			otherTeamPlanColor = ToolBox.Instance.LeftTeamColor;
        }
        else {
            Debug.Log(leftGoal+ " rightGoal " + rightGoal);
            activePlanTimeColor = ToolBox.Instance.LeftTeamColor;
			otherTeamPlanColor = ToolBox.Instance.RightTeamColor;
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
		if (ToolBox.Instance.MatchOver){ return; }
		else if (gameTimer.NoRemainingTime()){
                //adds overtime or ends the game
				OutOfGametimeCheck();

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
					UnpauseGameCoroutine = StartCoroutine(UnpauseGame());

					Debug.Log("unpausing since time ran out");
				}
            }
            else if (Input.GetKeyDown(KeyCode.Return)){
				EndTurn();
            }
			//if we are server, unpause the server and tell the other client to unpause
            else if (remoteIsReady && localIsReady && customIsServer && paused == true){
                server.SendUnpauseGame();
                UnpauseGameCoroutine = StartCoroutine(UnpauseGame());
            }
        }
    }
    /// <summary>
    /// assuming the same thing happens on both clients nothing needs to be sent
    /// over the network
    /// </summary>
    void OnScore()
    {
        //if(customIsServer == true){
             //pause game
            if(gameTimerCoroutine != null)
                StopCoroutine(gameTimerCoroutine);
            PauseGame();
        //}
    }
  
    public void PreOnGoalScored(){
        StopCoroutine(gameTimerCoroutine);
    }
    IEnumerator CountDownPlanningTime(){
        while(playerTurnhandler.currentPlanTimeLeft > 0 ){
            yield return new WaitForSecondsRealtime(1f);
            playerTurnhandler.currentPlanTimeLeft--;
        }
    }
      void UpdateTimerTexts(){
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
        if(gameTimeText != null){
            gameTimeText.text = zeroBeforeMin + gameTimer.MinutesRemaining() + ":" + zeroBeforeSec + gameTimer.SecondsRemaining();
        }
        if(planTimeText != null && paused == true){
           planTimeText.text = "" + (int)playerTurnhandler.currentPlanTimeLeft;
        }
    }
      void OutOfGametimeCheck(){
		if (gameTimer.NoRemainingTime()){
			if (leftGoal.score == rightGoal.score && !gameTimer.InOvertime && overTime > 0){
				gameTimer.AddOvertime(overTime);
                gameTimeText.color = ToolBox.Instance.MatchTimeWhenOvertimeColor;
                PauseGame();
            } else {
				handleMatchEnd = StartCoroutine(MatchEnd());
			}
		}
	}
    void StopCoroutineIfNotNull(Coroutine coroutine){
		if (coroutine != null){
			StopCoroutine(coroutine);
		}
	}
    IEnumerator MatchEnd(){
        paused = false;
        ToolBox.Instance.MatchOver = true;

        StopCoroutineIfNotNull(UnpauseGameCoroutine);
        StopCoroutineIfNotNull(countDownPlanningTime);
        StopCoroutineIfNotNull(gameTimerCoroutine);
      
        playerTurnhandler.PauseGame();
        otherTurnhandler.PauseGame();
        ball.Pause();

		if (leftGoal.score > rightGoal.score){
               endOfMatchAnim.SetTrigger("RightWin");
		}
		else if (rightGoal.score > leftGoal.score){
            endOfMatchAnim.SetTrigger("LeftWin");
		}
		else if(rightGoal.score == leftGoal.score){
            endOfMatchAnim.SetTrigger("Draw");
		}
        yield return new WaitForSeconds(5f);

        ToolBox.Instance.MatchOver = false; //Resets it on scene change since the ToolBox is persistent
        SceneManager.LoadScene("MainMenu");
        
    }
    public IEnumerator UnpauseGame()
    {
        if(paused == false){
            Debug.Log("breaking from unpause");
            yield break;
        }
        playerTurnhandler.Activate(false);
        Time.timeScale = 1;
        paused = false;
        StopCoroutine(countDownPlanningTime);
       
        ball.Unpause();
        playerTurnhandler.UnpauseGame();
        otherTurnhandler.UnpauseGame();
        localIsReady = false;
        remoteIsReady = false;
       
        gameTimerCoroutine = StartCoroutine(gameTimer.CountDownSeconds(roundTime));
        
		if (gameTimer.remainingTime <= roundTime && !gameTimer.InOvertime)
			animatingOvertime = true;

        yield return new WaitForSeconds(roundTime);

		if (animatingOvertime){
			overtimeAnimScript.StartAnimation(this);
		} else if (gameTimer.remainingTime <= roundTime && gameTimer.InOvertime){ //If in overtime and last turn we don't want a new turn
		} else {
            
			PauseGame();
		}
    }
    public void PauseGame(){
        if(paused== true){
            Debug.Log("game already paused, returning");
            return;
        }
        Debug.Log("pausing!");
        StopCoroutine(UnpauseGameCoroutine);
        StopCoroutine(gameTimerCoroutine);
        playerTurnAnim.SetTrigger(localPlayerSide);
        paused = true;
        commandsSent = false;
        server.recivedCommands = false;
        
        planTimeText.color = activePlanTimeColor;
        playerTurnhandler.currentPlanTimeLeft = planTime;
        playerTurnhandler.PauseGame();
        otherTurnhandler.PauseGame();
        
        ball.Pause();
        
        //if we are server
        if(customIsServer == true){
            List<GameObject> allRobots = new List<GameObject>();
            allRobots.AddRange(playerTurnhandler.Robots);
            allRobots.AddRange(otherTurnhandler.Robots);
            server.SendSyncStateMsg(allRobots, ball.gameObject, new Position(leftGoal.score,rightGoal.score), gameTimer.remainingTime);
        }
        
    }

	public void LeftTurnAnimCallback(){
		//Called after turn animation finishes, start counting down plantime here
        countDownPlanningTime = StartCoroutine(CountDownPlanningTime());
        AllowTurnEnd(true);
        ActivatePlayerTurnhandler(true);
	}

	public void RightTurnAnimCallback(){
		//Called after turn animation finishes, start counting down plantime here
        countDownPlanningTime = StartCoroutine(CountDownPlanningTime());
        AllowTurnEnd(true);
        ActivatePlayerTurnhandler(true);
	}
    void ActivatePlayerTurnhandler(bool value){
        playerTurnhandler.Activate(value);
    }
	public void OvertimeAnimCallback(){
        animatingOvertime = false;  
	}
    void AllowTurnEnd(bool allow){
		endTurnButton.interactable = allow; //Visually disables End Turn button (also functionally disables the button, but it's already disabled by the boolean)
		allowTurnEnd = allow;
	}
	public void EndTurn(){ //Called through button
		if (paused == true && localIsReady == false && allowTurnEnd){
			SendCommands();
			localIsReady = true;
			//change color of the plantime to show that we are waiting for the other player
			planTimeText.color = otherTeamPlanColor;
            playerTurnhandler.Activate(false);
            AllowTurnEnd(false);
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
       server.SendCommands(scList);
    }

    //Message callbacks
    public void OnRecieveUnpause(NetworkMessage netMsg)
    {
        Debug.Log("unpause msg rec!");
        if(paused == true)
            UnpauseGameCoroutine = StartCoroutine(UnpauseGame());
    }
    /// <summary>
    /// Recieve and put the commands into the otherTurnhandler's robots
    /// Also means that the other player is ready
    /// </summary>
    /// <param name="netMsg"></param>
    public void OnRecieveCommands(NetworkMessage netMsg){
        if (paused && server.recivedCommands == false){
            List<SerializableCommand> deserializedCommands = new List<SerializableCommand>();
            BinaryFormatter bf = new BinaryFormatter();
            Byte[] buffer = netMsg.ReadMessage<ServerBehaviour.CommandMsg>().serializedCommands;
            System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer);
            deserializedCommands = bf.Deserialize(ms) as List<SerializableCommand>;

            PutCommandsIntoRobots(deserializedCommands);

            remoteIsReady = true;
        }
        else {
            Debug.Log("game is paused: " + paused);
        }
    }

    public IEnumerator OnRecieveSyncStateCoroutine(NetworkMessage netMsg){
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
            PauseGame();
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
                        //OnScore();
                    }
                    //last is the gametime
                    gameTimer.remainingTime = (int)deserializedBuffer[i+3].x;

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

    public void DeselectRobot(){
        playerTurnhandler.THDeselectRobot();
    }

    public void SelectCommand(Command.AvailableCommands c){
        playerTurnhandler.THSelectCommand(c);
    }
}