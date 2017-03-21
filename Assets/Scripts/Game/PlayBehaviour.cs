using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayBehaviour : MonoBehaviour, IPlayBehaviour { //class for local play

	/// <summary>
	/// length of a planning -> play round
	/// </summary>
	public int roundTime;
	/// <summary>
	/// length of a whole match
	/// </summary>
	public int matchTime;
	/// <summary>
	/// length of a player's planing phase
	/// </summary>
	public float RoundTime { get { return roundTime; } }

	/// length of overtime to be added ONCE
	/// </summary>
	public int overTime;

	/// <summary>
	/// length of planning time each player has
	/// </summary>
	public int planTime;


	int roundCount = 0; //number of rounds played
	bool paused = true;
	bool isTH1Done = false;
	bool isTH2Done = false;

	Text gameTimeText;
	Text planTimeText;

	Ball ball;
	Goal leftGoalScript;
	Goal rightGoalScript;
	GameTimer gameTimer;
	Coroutine unpauseGame;
	Coroutine handleMatchEnd;
	Coroutine countDownPlanningTime;
    Coroutine gameTimerCoroutine;
	[SerializeField]
	TurnHandlerBehaviour turnHandler1;
	[SerializeField]
	TurnHandlerBehaviour turnHandler2;
	TurnHandlerBehaviour currentActiveTurnhandler;
	Animator endOfMatchAnim, playerTurnAnim;

	void Start(){
		Physics.queriesHitTriggers = true;
		ball = GameObject.Find("Ball").GetComponent<Ball>();
		leftGoalScript = GameObject.Find("LeftGoal").GetComponent<Goal>();
		rightGoalScript = GameObject.Find("RightGoal").GetComponent<Goal>();
        endOfMatchAnim = GameObject.Find("EndOfMatchAnimation").GetComponent<Animator>();
		playerTurnAnim = GameObject.Find("PlayerTurnAnimation").GetComponent<Animator>();

		//event callbacks for scoring
		if (leftGoalScript != null || rightGoalScript != null){
			Goal.OnGoalScored += new Goal.GoalScored(OnScore); 
		}
		else {
			Debug.Log("couldint find goals :(");
		}
		gameTimer = new GameTimer(matchTime);
		if (gameTimeText == null){
			gameTimeText = GameObject.Find("GameTimeText").GetComponent<Text>();
		}
		if (planTimeText == null){
			planTimeText = GameObject.Find("PlanTimeText").GetComponent<Text>();
		}
		UpdateTimerTexts();
		NewTurn();
	}

    void UpdateTimerTexts(){
		string zeroBeforeMin;
		string zeroBeforeSec;
		if (gameTimer.MinutesRemaining() < 10)
			zeroBeforeMin = "0";
		else
			zeroBeforeMin = "";
		if (gameTimer.SecondsRemaining() < 10)
			zeroBeforeSec = "0";
		else
			zeroBeforeSec = "";

		if (gameTimeText != null){
			gameTimeText.text = zeroBeforeMin + gameTimer.MinutesRemaining() + ":" + zeroBeforeSec + gameTimer.SecondsRemaining();
		}
		if (planTimeText != null && currentActiveTurnhandler != null && paused == true){
			if (currentActiveTurnhandler == turnHandler2)
				planTimeText.color = ToolBox.Instance.RightTeamColor;
			else
				planTimeText.color = ToolBox.Instance.LeftTeamColor;
			planTimeText.text = "" + (int)currentActiveTurnhandler.CurrentPlanTimeLeft;
		}
	}

	void OnScore(){
		//tell gametimer and the unpause to stop
		StopAllCoroutines();
        //do waht unpause does at the end

        currentActiveTurnhandler = null;
		NewTurn();
		PauseGame();
		//robots reset their position by listening to the same event
	}

	void OnDestroy(){
		StopAllCoroutines();
		Goal.OnGoalScored -= new Goal.GoalScored(OnScore); 
	}

	/// <summary>
	/// decreases the plantime of the currentactiveturnhandler
	/// </summary>
	/// <returns></returns>
	IEnumerator CountDownPlanningTime(){
		while(currentActiveTurnhandler.CurrentPlanTimeLeft > 0){ 
			yield return new WaitForSecondsRealtime(1f);
			currentActiveTurnhandler.CurrentPlanTimeLeft--;
		}
	}

	void StopCoroutineIfNotNull(Coroutine coroutine){
		if (coroutine != null){
			StopCoroutine(coroutine);
		}
	}

	void Update(){
		if (gameTimer.IsGameOver()){ //Stops the game when time runs out ALSO STOPS OVERTIME - BUG
			if (!ToolBox.Instance.MatchOver){ //Only ran first time IsGameOver returns true
				UpdateTimerTexts();
				MatchEnd();
			}
			return;
		}
		UpdateTimerTexts();
		//restrict the amount of time a player has to plan
		if (paused == true && currentActiveTurnhandler != null){
			//if the player still hasn't run out of time, decrease it!
			if(currentActiveTurnhandler.CurrentPlanTimeLeft <= 0){ 
				//if the player ran out of time, set them to ready
				Debug.Log("current player ran out of plan time, setting it to ready");

				if (currentActiveTurnhandler == turnHandler1){
					isTH1Done = true;
				}
				else if (currentActiveTurnhandler == turnHandler2){
					isTH2Done = true;
				}
				StopCoroutineIfNotNull(countDownPlanningTime);
				ChooseNextCurrentTurnHandler();
				ActivateTurnHandler(true);
			}
		}
		//listen for buttonpresses like wanting to send your move
		if (Input.GetKeyDown(KeyCode.Return) && paused == true){
			if (currentActiveTurnhandler == turnHandler1){
				Debug.Log("1 is ready");
				isTH1Done = true;
			}
			else if (currentActiveTurnhandler == turnHandler2){
				Debug.Log("2 is ready");
				isTH2Done = true;
			}
			StopCoroutineIfNotNull(countDownPlanningTime);
			// if someone still isnt done then give control to that player
			if (!isTH1Done || !isTH2Done){
				ChooseNextCurrentTurnHandler();
				ActivateTurnHandler(true);
			}
		}
		// are both players done?
		if (isTH1Done && isTH2Done){
			StopCoroutineIfNotNull(countDownPlanningTime);
			//then play the game and pause again in roundTime seconds

			unpauseGame = StartCoroutine(UnpauseGame());
			isTH1Done = false;
			isTH2Done = false;
		}
	}

	/// <summary>
	/// Displays who the winner was and goes back to the main menu.
	/// </summary>
	void MatchEnd(){
		paused = false;
		ToolBox.Instance.MatchOver = true;
		//check if the score is tied, then add overtime (if not already overtime) and continue
		if (leftGoalScript.score == rightGoalScript.score && gameTimer.InOvertime() == false && overTime > 0){
			Debug.Log("show that overtime is happening!!");

			gameTimer.AddOvertime(overTime);
		}
		else { //if possible, display winner!
			PauseGame();
			if (leftGoalScript.score > rightGoalScript.score){
                endOfMatchAnim.SetTrigger("RightWin");
				Debug.Log("left team won!");
			}
			else if (rightGoalScript.score > leftGoalScript.score){
                endOfMatchAnim.SetTrigger("LeftWin");
				Debug.Log("right team won!");
			}
			else if(rightGoalScript.score == leftGoalScript.score){
                endOfMatchAnim.SetTrigger("Draw");
                Debug.Log("match was a draw!");
			}
		}
	}

	//If we replayed the last turn, we dont want to do the newturn stuff
	public IEnumerator UnpauseGame(){
		if(paused == false){
			Debug.Log("game already unpaused, breaking");
			yield break;
		}
		Debug.Log("GAME IS UNPAUSED!");
        
        paused = false;
		Time.timeScale = 1;
		ball.Unpause();

		ActivateTurnHandler(false);
		planTimeText.enabled = false;

		turnHandler1.UnpauseGame();
		turnHandler2.UnpauseGame();

        gameTimerCoroutine = StartCoroutine(gameTimer.CountDownSeconds((int)roundTime));

		yield return new WaitForSeconds(roundTime);

		currentActiveTurnhandler = null;
		NewTurn();

		PauseGame();
	}

	public void PauseGame(){
		if(paused== true){
            Debug.Log("game already paused, returning");
            return;
        }
		ball.Pause();
		planTimeText.enabled = true;
		turnHandler1.PauseGame();
		turnHandler2.PauseGame();
		paused = true;
	}

	/// <summary>
	/// Gives or takes control from the current turnhandler activate true means to activate current turnhandler,
	/// also counts down the planning time activate false means to deactivate current turnhandler.
	/// </summary>
	void ActivateTurnHandler(bool activate){
		turnHandler1.gameObject.SetActive(true);
		turnHandler2.gameObject.SetActive(true);

		if (activate){
			if (currentActiveTurnhandler == turnHandler1 && !isTH1Done){ //gives control to the current turnhandler
				turnHandler1.Activate(true);
				countDownPlanningTime = StartCoroutine(CountDownPlanningTime());
				turnHandler2.Activate(false); //deactivates the other one
			}
			else if (currentActiveTurnhandler == turnHandler2 && !isTH2Done){
				turnHandler2.Activate(true);
				countDownPlanningTime =  StartCoroutine(CountDownPlanningTime());
				turnHandler1.Activate(false); //deactivates the other one
			}   
		}
		else {
			turnHandler1.Activate(false);
			turnHandler2.Activate(false);
		}
	}

	void NewTurn(){
		roundCount++;
		if (roundCount % 2 == 0){
			currentActiveTurnhandler = turnHandler1;
			turnHandler1.CurrentPlanTimeLeft = planTime;
		}
		else {
			currentActiveTurnhandler = turnHandler2;
			turnHandler2.CurrentPlanTimeLeft = planTime;
		}
		turnHandler1.CurrentPlanTimeLeft = planTime;
		isTH1Done = false;

		turnHandler2.CurrentPlanTimeLeft = planTime;
		isTH2Done = false;

		ActivateTurnHandler(true);
	}

	void ChooseNextCurrentTurnHandler(){
		if (currentActiveTurnhandler == turnHandler1 && isTH1Done && isTH2Done == false){
			currentActiveTurnhandler = turnHandler2;
		}
		else if (currentActiveTurnhandler == turnHandler2 && isTH2Done && isTH1Done == false){
			currentActiveTurnhandler = turnHandler1;
		}
		else if (isTH1Done && isTH2Done){
			currentActiveTurnhandler = null;
		}
	}

	//Stuff below passes functions through to the turn handlers
	public void DeselectRobot(){
		currentActiveTurnhandler.THDeselectRobot();
	}
	public void SelectCommand(Command.AvailableCommands command){
		currentActiveTurnhandler.THSelectCommand(command);
	}

    public void PreOnGoalScored()
    {
        StopCoroutine(unpauseGame);
        StopCoroutine(gameTimerCoroutine);
    }
}