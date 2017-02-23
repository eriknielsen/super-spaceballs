using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayBehaviour : MonoBehaviour { //class for local play
    
    [SerializeField]
    TurnHandlerBehaviour turnHandler1;
    [SerializeField]
    TurnHandlerBehaviour turnHandler2;

	private bool isTH1Done = false;
	private bool isTH2Done = false;
    private int currentTurnHandler;
	private GameTimer gameTimer;

	public float gameTime;
    public Text gameTimeText;
    public float roundTime;
	public static float RoundTime { get { return Instance.roundTime; } }
	public float intendedShockwaveLiftime;

    public Goal leftGoal;
    public Goal rightGoal;

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
            leftGoal.OnGoalScored += new Goal.GoalScored(() => leftGoal.score++);
            rightGoal.OnGoalScored += new Goal.GoalScored(() => rightGoal.score++);
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

    void Update(){
        gameTimeText.text = "Time " + gameTimer.MinutesRemaining() + ":" + gameTimer.SecondsRemaining();

        //listen for buttonpresses like wanting to send your move etc
        if (Input.GetKeyDown(KeyCode.Return)){
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

        ActivateTurnHandler(false);
        if (asReplay){
            turnHandler1.ReplayLastTurn();
            turnHandler2.ReplayLastTurn();
        }

        turnHandler1.UnpauseGame();
        turnHandler2.UnpauseGame();
        StartCoroutine(gameTimer.CountDownSeconds((int)roundTime));
        yield return new WaitForSeconds(roundTime);
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
        turnHandler1.PauseGame();
        turnHandler2.PauseGame();
        //TurnOffColliders();
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

//    public void ReplayLastTurn(){
//        Debug.Log("replay?");
//        if (turnHandler1.Turns > 1){
//            //unpause game calls the necessary replayturn functions if we
//            //send the argument as true
//            StartCoroutine(UnpauseGame(true));
//        }
//        else {
//            Debug.Log("there needs to be at least one turn");
//        }
//    }

	//Stuff below passes functions through to the turn handlers, because our code structure is shit :D
	public void DeselectRobot(){
		turnHandler1.THDeselectRobot();
		turnHandler2.THDeselectRobot();
	}

	public void SelectedCommand(Command.AvailableCommands command){ 
		turnHandler1.THSelectCommand(command);
		turnHandler2.THSelectCommand(command);
	}
}