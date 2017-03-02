using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayBehaviour : MonoBehaviour { //class for local play
    
	private bool isTH1Done = false;
	private bool isTH2Done = false;
    private int currentTurnHandler;
	private GameTimer gameTimer;

   
    TurnHandlerBehaviour turnHandler1;
    
    TurnHandlerBehaviour turnHandler2;
    bool isGamePaused = true;

    Text gameTimeText;
    /// <summary>
    /// length of a planning -> play round
    /// </summary>
    public int roundTime;
    /// <summary>
    /// length of a whole match
    /// </summary>
    public int matchTime;
    /// <summary>
    /// length of overtime to be added ONCE
    /// </summary>
    public int overTime;
	public static float RoundTime { get { return Instance.roundTime; } }


    Goal leftGoal;
    Goal rightGoal;

    public delegate void GamePaused();
    public static event GamePaused OnPauseGame;
    public delegate void PreGamePaused();
    public static event PreGamePaused PreOnPauseGame;
    public delegate void GameUnpaused();
    public static event GameUnpaused OnUnpauseGame;

    static PlayBehaviour instance;

    public static PlayBehaviour Instance
    {
        get {
            return instance; 
        }
    }

    void Awake(){
		if (instance == null)
            instance = this;
		else if (instance != this) { //Makes sure there's only one instance of the script
			Debug.Log("There's already an instance of PlayBehaviour");
			Destroy(gameObject); //Goes nuclear
		}
		Physics.queriesHitTriggers = true;
    }

    void Start(){
        leftGoal = GameObject.Find("LeftGoal").GetComponent<Goal>();
        rightGoal = GameObject.Find("RightGoal").GetComponent<Goal>();
        turnHandler1 = GameObject.Find("TurnHandlerLeft").GetComponent<TurnHandlerBehaviour>();
        turnHandler2 = GameObject.Find("TurnHandlerRight").GetComponent<TurnHandlerBehaviour>();
        //event callbacks for scoring
        if (leftGoal != null || rightGoal != null){
            Goal.OnGoalScored += new Goal.GoalScored(OnScore);
            
        }
        else {
            Debug.Log("couldint find goals :(");
        }
        gameTimer = new GameTimer(matchTime);
        if(gameTimeText == null)
        {
            gameTimeText = GameObject.Find("GameTimeText").GetComponent<Text>();
        }
        gameTimeText.text = "Time " + gameTimer.MinutesRemaining() + ":" + gameTimer.SecondsRemaining();

        NewTurn();
    }

    void OnScore()
    {
        //tell gametimer and the unpause to stop
        StopAllCoroutines();
        //do waht unpause does at the end
        currentTurnHandler = -1;
        NewTurn();
        //wait a bit so that the robot animations can be set to idle
        Invoke("PauseGame",0.02f);
        //robots reset their position by themselves
    }
    /// <summary>
    /// displays who the winner was and goes back to the main menu
    /// </summary>
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
                Debug.Log("left team won!");
            }
            //right won! 
            else if (rightGoal.score > leftGoal.score)
            {
                Debug.Log("right team won!");
            }
            else if(rightGoal.score == leftGoal.score)
            {
                Debug.Log("match was even!");
            }
            //wait a bit and then change scene to mainmenu
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("MainMenu");
        }
    }
    void Update(){
        //if gametime has run out, do stuff!
        if (gameTimer.IsGameOver())
        {
                StartCoroutine(HandleMatchEnd());
        }
        
        gameTimeText.text = "Time " + gameTimer.MinutesRemaining() + ":" + gameTimer.SecondsRemaining();

        //listen for buttonpresses like wanting to send your move etc
        if (Input.GetKeyDown(KeyCode.Return) && isGamePaused){
            if (currentTurnHandler == 1){
                isTH1Done = true;
            }
            else if (currentTurnHandler == 2){
                isTH2Done = true;
            }
            // are both players done?
            if (isTH1Done && isTH2Done){
                //then play the game and pause again in 4 seconds
                StartCoroutine(UnpauseGame(false));
            }
            //decides who plays next if both players arent done
            else {
                ChooseNextCurrentTurnHandler();
                ActivateTurnHandler(true);
            }
        }
    }
    //if we replayed the last turn, we dont want to do the newturn stuff
    IEnumerator UnpauseGame(bool asReplay){
        isGamePaused = false;
        if(OnUnpauseGame != null)
            OnUnpauseGame();
        ActivateTurnHandler(false);
        if (asReplay){
            turnHandler1.ReplayLastTurn();
            turnHandler2.ReplayLastTurn();
        }

        turnHandler1.UnpauseGame();
        turnHandler2.UnpauseGame();
        StartCoroutine(gameTimer.CountDownSeconds((int)roundTime));

        yield return new WaitForSeconds(roundTime-Time.deltaTime);
        if(PreOnPauseGame != null)
            PreOnPauseGame();
        yield return new WaitForSeconds(Time.deltaTime);
        if (asReplay == false){
            currentTurnHandler = -1;
            NewTurn();
        }
        else {
            //if we just replayed the turn, we have to put
            // the commands they had before the replay back
            turnHandler1.RevertToOldCommands();
            turnHandler2.RevertToOldCommands();
            ActivateTurnHandler(true);
        }
        PauseGame();
    }

    void PauseGame(){
        isGamePaused = true;
        if(OnPauseGame != null)
            OnPauseGame();
        turnHandler1.PauseGame();
        turnHandler2.PauseGame();
    }

    /// <summary>
    /// gives or takes control from the current turnhandler
    /// activate true means to activate current turnhandler
    /// activate false means to deactivate current turnhandler
    /// </summary>
    void ActivateTurnHandler(bool activate){
        //TurnOnColliders();
		turnHandler1.gameObject.SetActive(true);
		turnHandler2.gameObject.SetActive(true);

        if (activate){
            //gives control to the current turnhandler
            if (currentTurnHandler == 1 && !isTH1Done){
                turnHandler1.Activate(true);
                Debug.Log("activating1");
                //make sure to deactivate the other one
                turnHandler2.Activate(false);

            }
            else if (currentTurnHandler == 2 && !isTH2Done){
                turnHandler2.Activate(true);
                Debug.Log("activating2");
                //make sure to deactivate the other one
                turnHandler1.Activate(false);
            }
        }
        else {
            turnHandler1.Activate(false);
            turnHandler2.Activate(false);
        }
    }

    void NewTurn(){
        if (turnHandler1.Turns % 2 == 0){
            currentTurnHandler = 1;
        }
        else {
            currentTurnHandler = 2;
        }
        isTH1Done = false;
        isTH2Done = false;

        ActivateTurnHandler(true);
    }

    void ChooseNextCurrentTurnHandler(){
        //if current is one and finished AND other one isnt done, 
        //give control to the other one
        if (currentTurnHandler == 1 && isTH1Done && isTH2Done == false){
            currentTurnHandler = 2;
        }
        else if (currentTurnHandler == 2 && isTH2Done && isTH1Done == false){
            currentTurnHandler = 1;
        }
        else if (isTH1Done && isTH2Done){
            currentTurnHandler = -1;
        }
    }
    /// Pauses the game and deactivates ui and active turnhandler or activates ui and active turnhandler
    /// Called by GameBehaviour
    public void Activate(bool activate){ //REMOVE FUNCTION?
        if (activate == false){
            //DestroyTurnHandlers();
     
            enabled = false;
        }
        else {
            enabled = true;
            //CreateTurnHandlers();
            NewTurn();
        }
    }


	//Stuff below passes functions through to the turn handlers, because our code structure is shit :D
	public void DeselectRobot(){
		if (currentTurnHandler == 1) {
			turnHandler1.THDeselectRobot ();
		}
		if (currentTurnHandler == 2) {
			turnHandler2.THDeselectRobot ();
		}
	}

	public void SelectCommand(Command.AvailableCommands command){
		if (currentTurnHandler == 1) {
			turnHandler1.THSelectCommand (command);
		}
		if (currentTurnHandler == 2) {
			turnHandler2.THSelectCommand (command);
		}
	}
}