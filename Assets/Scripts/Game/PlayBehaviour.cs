using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayBehaviour : MonoBehaviour {
    //class for local play


    TurnHandlerBehaviour turnHandler1;
    TurnHandlerBehaviour turnHandler2;
    bool isTH1Done = false;
    bool isTH2Done = false;
    int currentTurnHandler;
    Vector2 score;
    GameTimer gameTimer;

    public Text gameTimeText;
    public GameObject InGameUIInstance;

    public float roundTime;
    public GameObject InGameUIPrefab;
    public GameObject turnHandlerPrefab;

   
    public delegate void ReturnMenuButtonClicked();
    public static event ReturnMenuButtonClicked OnReturnMenuButtonClick;

    public delegate void ReplayButtonClicked();
    public static event ReplayButtonClicked OnReplayButtonClick;

    void Awake()
    {
        gameTimer = new GameTimer(120);
        InGameUIInstance = Instantiate(InGameUIPrefab);
        turnHandler1 = transform.FindChild("TurnHandlerLeft").GetComponent<TurnHandlerBehaviour>();
        turnHandler2 = transform.FindChild("TurnHandlerRight").GetComponent<TurnHandlerBehaviour>();
        gameTimeText = InGameUIInstance.transform.FindChild("GameTimeText").GetComponent<Text>();
      
    }
    // Use this for initialization
    void Start () {
        transform.FindChild("TurnHandlerRight").gameObject.SetActive(true);
        transform.FindChild("TurnHandlerLeft").gameObject.SetActive(true);
        InGameUIInstance.SetActive(true);

        //decide who goes first
        NewTurn();
        gameTimeText.text = "Time " + gameTimer.MinutesRemaining() + ":" + gameTimer.SecondsRemaining();


    }

    // Update is called once per frame
    void Update () {
        Debug.Log(gameTimer.SecondsRemaining());
        gameTimeText.text = "Time " + gameTimer.MinutesRemaining() + ":" + gameTimer.SecondsRemaining();
       
        //listen  for buttonpresses like wanting to send your move etc
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (currentTurnHandler == 1)
            {
                isTH1Done = true;     
            }
            else if(currentTurnHandler == 2)
            {
                isTH2Done = true;
            }
            // are both players done?
            if (isTH1Done && isTH2Done)
            {
                //then play the game and pause again in 4 seconds
                StartCoroutine(UnpauseGame(false));
            }
            //decides who plays next if both players arent done
            else
            {
                ChooseNextCurrentTurnHandler();
                ActivateCorrectTurnHandler(true);
            }   
        }
    }
    ///if we replayed the last turn, then we dont want to do the newturn stuff
    IEnumerator UnpauseGame(bool asReplay)
    {
        InGameUIInstance.transform.GetChild(0).gameObject.SetActive(false);
        InGameUIInstance.transform.GetChild(1).gameObject.SetActive(false);

        ActivateCorrectTurnHandler(false);
        if (asReplay)
        {
            turnHandler1.ReplayLastTurn();
            turnHandler2.ReplayLastTurn();
        }
        
        turnHandler1.UnpauseGame();
        turnHandler2.UnpauseGame();
        StartCoroutine(gameTimer.CountDownSeconds(4));
        yield return new WaitForSeconds(4f);
        if(asReplay == false)
        {
            currentTurnHandler = -1;
            NewTurn();
        }
        else
        {
            //if we just replayed the turn, we have to put
            // the commands they had before the replay back
            turnHandler1.RevertToOldCommands();
            turnHandler2.RevertToOldCommands();
            ActivateCorrectTurnHandler(true);
        }            
        PauseGame();
        
        
    }
    void PauseGame()
    {
        InGameUIInstance.transform.GetChild(0).gameObject.SetActive(true);
        InGameUIInstance.transform.GetChild(1).gameObject.SetActive(true);

        turnHandler1.PauseGame();
        turnHandler2.PauseGame();
    }
    /// <summary>
    /// gives or takes control from the current turnhandler
    /// activate true means to activate current turnhandler
    /// activate false means to deactivate current turnhandler
    /// </summary>
    void ActivateCorrectTurnHandler(bool activate)
    {
        
        if (activate)
        {
            //gives control to the current turnhandler
            if (currentTurnHandler == 1 && !isTH1Done)
            {
                turnHandler1.Activate(true);
                Debug.Log("activating1");
                //make sure to deactivate the other one
                turnHandler2.Activate(false);
             
            }
            else if (currentTurnHandler == 2 && !isTH2Done)
            {
                turnHandler2.Activate(true);
                Debug.Log("activating2");
                //make sure to deactivate the other one
                turnHandler1.Activate(false);
            }
        }
        else
        {
            if(currentTurnHandler == 1)
            {
                turnHandler1.Activate(false);
            }
            else if(currentTurnHandler == 2)
            {
                turnHandler2.Activate(false);
            }
        }
    }
    void NewTurn()
    {
        if(turnHandler1.Turns % 2 == 0)
        {
            currentTurnHandler = 1;
            
        }
        else
        {
            currentTurnHandler = 2;
            
        }
        isTH1Done = false;
        isTH2Done = false;

        ActivateCorrectTurnHandler(true);
    }
    void ChooseNextCurrentTurnHandler()
    {
        //if current is one and finished AND other one isnt done, 
        //give control to the other one
        if(currentTurnHandler == 1 && isTH1Done && isTH2Done == false)
        {
            currentTurnHandler = 2;
        }
        else if(currentTurnHandler == 2 && isTH2Done && isTH1Done == false)
        {
            currentTurnHandler = 1;  
        }
        else if(isTH1Done && isTH2Done)
        {
            currentTurnHandler = -1;
        }
    }
    /// <summary>
    /// this either pauses the game and deactivates ui and active turnhandler
    ///  or it activates ui and active turnhandler
    ///  Is called by GameBehaviour
    /// </summary>
    /// <param name="activate"></param>
    public void Activate(bool activate)
    {
        if(activate == false)
        {
            
            //DEACTIVATE CURRENT TURNHANDLER
            ActivateCorrectTurnHandler(false);
            PauseGame();
            InGameUIInstance.SetActive(false);
            enabled = false;

        }
        else
        {
            enabled = true;
            ActivateCorrectTurnHandler(true);
            InGameUIInstance.SetActive(true);
        }
        
    }
    /// <summary>
    /// sends event to gamebehaviour about pausing the game
    ///  and showing the main menu
    /// </summary>
    public void PressReturnToMenu()
    {
        //game behaviour calls Activate(false) in here
        OnReturnMenuButtonClick();
        
    }

    public void ReplayLastTurn()
    {
        Debug.Log("replay?");
        if(turnHandler1.Turns > 1)
        {
            //unpause game calls the necessary replayturn functions if we
            //send the argument as true
            StartCoroutine(UnpauseGame(true));
        }
        else
        {
            Debug.Log("there needs to be at least one turn");
        } 
    }
    public void ReplayButtonClick()
    {
        OnReplayButtonClick();
    }
}
