using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayBehaviour : MonoBehaviour, IPlayBehaviour { //class for local play
    
	private bool isTH1Done = false;
	private bool isTH2Done = false;
    private bool paused = true;
    //private int currentTurnHandler;
	private GameTimer gameTimer;

    [SerializeField]
    TurnHandlerBehaviour turnHandler1;
    [SerializeField]
    TurnHandlerBehaviour turnHandler2;
    TurnHandlerBehaviour currentActiveTurnhandler;

    Text gameTimeText;
    Text planTimeText;
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
    public int planTime;
	public float RoundTime { get { return roundTime; } }

    //number of rounds played
    int roundCount = 0;

    Goal leftGoal;
    Goal rightGoal;
    Ball ball;

    Coroutine countDownCoroutineInstance;

    void Awake(){
		Physics.queriesHitTriggers = true;
    }

    void Start(){
        leftGoal = GameObject.Find("LeftGoal").GetComponent<Goal>();
        rightGoal = GameObject.Find("RightGoal").GetComponent<Goal>();
        ball = GameObject.Find("Ball").GetComponent<Ball>();

        //event callbacks for scoring
        if(leftGoal != null || rightGoal != null){
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

        if(planTimeText == null)
        {
            planTimeText = GameObject.Find("PlanTimeText").GetComponent<Text>();
        }
        planTimeText.text = "Plan time: " + planTime;
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
    /// <summary>
    /// decreases the plantime of the currentactiveturnhandler
    /// </summary>
    /// <returns></returns>
    IEnumerator CountDownPlanningTime()
    {
        while(currentActiveTurnhandler.currentPlanTimeLeft > 0)
        { 
            yield return new WaitForSecondsRealtime(1f);
            currentActiveTurnhandler.currentPlanTimeLeft--;
        } 
    }
    void Update(){
        gameTimeText.text = "Time " + gameTimer.MinutesRemaining() + ":" + gameTimer.SecondsRemaining();

        //restrict the amount of time a player has to plan
        if (paused == true && currentActiveTurnhandler != null)
        {
            planTimeText.text = "Plan time: " + 
                (int)currentActiveTurnhandler.currentPlanTimeLeft;
            //if the player still hasn't run out of time, decrease it!
            if(currentActiveTurnhandler.currentPlanTimeLeft <= 0) { 
                //if the player ran out of time, set them to ready
                Debug.Log("current player ran out of plan time, setting it to ready");
                
                if (currentActiveTurnhandler == turnHandler1)
                {
                    isTH1Done = true;
                }
                else if (currentActiveTurnhandler == turnHandler2)
                {
                    isTH2Done = true;
                }
                ChooseNextCurrentTurnHandler();
                ActivateTurnHandler(true);
                StopCoroutine(countDownCoroutineInstance);
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
            StopCoroutine(countDownCoroutineInstance);
            // if someone still isnt done then give control to that player
            if (!isTH1Done || !isTH2Done){
                ChooseNextCurrentTurnHandler();
                ActivateTurnHandler(true);
            }
        }
        // are both players done?
        if (isTH1Done && isTH2Done)
        {
            StopCoroutine(countDownCoroutineInstance);
            //then play the game and pause again in roundTime seconds
          
            StartCoroutine(UnpauseGame());
            isTH1Done = false;
            isTH2Done = false;
        }

    }
    //if we replayed the last turn, we dont want to do the newturn stuff
    IEnumerator UnpauseGame(){

        paused = false;
        ball.Unpause();
        ActivateTurnHandler(false);
        planTimeText.enabled = false;
     
        turnHandler1.UnpauseGame();
        turnHandler2.UnpauseGame();
        
        StartCoroutine(gameTimer.CountDownSeconds((int)roundTime));

        yield return new WaitForSeconds(roundTime);

       
        currentActiveTurnhandler = null;
        NewTurn();
        
        PauseGame();
    }

    void PauseGame(){

        ball.Pause();
        planTimeText.enabled = true;
        turnHandler1.PauseGame();
        turnHandler2.PauseGame();
        paused = true;
    }

    /// <summary>
    /// gives or takes control from the current turnhandler
    /// activate true means to activate current turnhandler, also countsdown the planning time
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
                countDownCoroutineInstance = StartCoroutine(CountDownPlanningTime());
                

                //make sure to deactivate the other one
                turnHandler2.Activate(false);

            }
            else if (currentActiveTurnhandler == turnHandler2 && !isTH2Done){
                turnHandler2.Activate(true);
                countDownCoroutineInstance =  StartCoroutine(CountDownPlanningTime());

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
        turnHandler1.currentPlanTimeLeft = planTime;
        isTH1Done = false;
       
        turnHandler2.currentPlanTimeLeft = planTime;
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