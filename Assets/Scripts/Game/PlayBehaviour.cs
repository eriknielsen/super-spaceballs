using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayBehaviour : MonoBehaviour, IPlayBehaviour { //class for local play
    
	private bool isTH1Done = false;
	private bool isTH2Done = false;
    //private int currentTurnHandler;
	private GameTimer gameTimer;

    [SerializeField]
    TurnHandlerBehaviour turnHandler1;
    [SerializeField]
    TurnHandlerBehaviour turnHandler2;
    TurnHandlerBehaviour currentActiveTurnhandler;

    Text gameTimeText;
    /// <summary>
    /// length of a planning -> play round
    /// </summary>
    public float roundTime;
    /// <summary>
    /// length of a whole match
    /// </summary>
    public float matchTime;
	public static float RoundTime { get { return Instance.roundTime; } }

    //number of rounds played
    int roundCount = 0;

    Goal leftGoal;
    Goal rightGoal;

    public delegate void GamePaused();
    public static event GamePaused OnPauseGame;
    public delegate void PreGamePaused();
    public static event PreGamePaused PreOnPauseGame;
    public delegate void GameUnpaused();
    public static event GameUnpaused OnUnpauseGame;

    public static PlayBehaviour Instance;

    void Awake(){
		if (Instance == null)
			Instance = this;
		else if (Instance != this) { //Makes sure there's only one instance of the script
			Debug.Log("There's already an instance of PlayBehaviour");
			Destroy(gameObject); //Goes nuclear
		}
		Physics.queriesHitTriggers = true;
    }

    void Start(){
        leftGoal = GameObject.Find("LeftGoal").GetComponent<Goal>();
        rightGoal = GameObject.Find("RightGoal").GetComponent<Goal>();
        //event callbacks for scoring
        if(leftGoal != null || rightGoal != null){
            Goal.OnGoalScored += new Goal.GoalScored(OnScore);
            
        }
        else {
            Debug.Log("couldint find goals :(");
        }
        gameTimer = new GameTimer(120);
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
        currentActiveTurnhandler = null;
        NewTurn();
        PauseGame();
        //robots reset their position by themselves
    }

    void Update(){
        gameTimeText.text = "Time " + gameTimer.MinutesRemaining() + ":" + gameTimer.SecondsRemaining();

        //listen for buttonpresses like wanting to send your move etc
        if (Input.GetKeyDown(KeyCode.Return)){
            if (currentActiveTurnhandler == turnHandler1){
                isTH1Done = true;
            }
            else if (currentActiveTurnhandler == turnHandler2){
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
        {
            PreOnPauseGame();
        }
        yield return new WaitForSeconds(Time.deltaTime);
        if (asReplay == false){
            currentActiveTurnhandler = null;
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
            if (currentActiveTurnhandler == turnHandler1 && !isTH1Done){
                turnHandler1.Activate(true);
                Debug.Log("activating1");
                //make sure to deactivate the other one
                turnHandler2.Activate(false);

            }
            else if (currentActiveTurnhandler == turnHandler2 && !isTH2Done){
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
        roundCount++;
        if (roundCount % 2 == 0){
            currentActiveTurnhandler = turnHandler1;
        }
        else {
            currentActiveTurnhandler = turnHandler2;
        }
        isTH1Done = false;
        isTH2Done = false;

        ActivateTurnHandler(true);
    }

    void ChooseNextCurrentTurnHandler(){
        //if current is one and finished AND other one isnt done, 
        //give control to the other one
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
}