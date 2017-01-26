using UnityEngine;
using System.Collections;

public class PlayBehaviour : MonoBehaviour {
    //class for local play

    public GameObject InGameUIPrefab;
    GameObject InGameUIInstance;

    public delegate void ReturnMenuButtonClicked();
    public static event ReturnMenuButtonClicked OnReturnMenuButtonClick;
    //instanitate turnhandlers, one for each team
    public GameObject turnHandlerPrefab;
    TurnHandlerBehaviour turnHandler1;
    TurnHandlerBehaviour turnHandler2;
    bool isTH1Done = false;
    bool isTH2Done = false;
    public int currentTurnHandler;

    void Awake()
    {
        
        InGameUIInstance = Instantiate(InGameUIPrefab);
    }
	// Use this for initialization
	void Start () {
      
        //turn handlers start "deactivated"
        turnHandler1 = Instantiate(turnHandlerPrefab).GetComponent<TurnHandlerBehaviour>();
        turnHandler2 = Instantiate(turnHandlerPrefab).GetComponent<TurnHandlerBehaviour>();
        InGameUIInstance.SetActive(true);

    }

    // Update is called once per frame
    void Update () {
        //listen  for buttonpresses like wanting to send your move etc
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if(currentTurnHandler == 1)
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
                //then play the game and pause in 4 seconds
                //UnpauseGame();
            }
            //decides who plays next
            GiveControlToNextPlayer();
        }
	}
    IEnumerator UnpauseGame()
    {
        Time.timeScale = 1;
        //rename to pauserobots?
        turnHandler1.UnpauseGame();
        turnHandler2.UnpauseGame();
        yield return new WaitForSeconds(4f);
        PauseGame();
        
    }
    void PauseGame()
    {
        
        Time.timeScale = 0;
        turnHandler1.PauseGame();
        turnHandler2.PauseGame();
        NewTurn();
    }
    /// <summary>
    /// gives control back to currentTurnhandler when coming back from menu
    /// </summary>
    void ActivateCorrectTurnHandler()
    {
        if(currentTurnHandler == 1 && !isTH1Done)
        {
            //turnHandler1.activate();
        }
        else if(currentTurnHandler == 2 && !isTH2Done)
        {
            //turnHandler2.activate();
        }
    }
    void NewTurn()
    {
        /*
        if(turnHandler1.turns % 2 == 0)
        {
            //turnHandler1.activate();
            currentTurnHandler = 1;
            isTH1Done = false;
        }
        else
        {
            //turnHandler2.activate();
            currentTurnHandler = 2;
            isTH2Done = false;
        }
        */
    }
    void GiveControlToNextPlayer()
    {
        if(currentTurnHandler == 1 && isTH1Done)
        {
            currentTurnHandler = 2;
            //turnhandler2.activate();
        }
        else if(currentTurnHandler == 2 && isTH2Done)
        {
            currentTurnHandler = 1;
            //turnhandler1.activate();
        }
    }
    public void Activate(bool b)
    {
        if(b == false)
        {
            //activate(false) would probably mean to go to the mainmenu but we dont want to destroy anything
            //so that we can go back to the game and play
            
            //DEACTIVATE CURRENT TURNHANDLER

            PauseGame();
            InGameUIInstance.SetActive(false);
            enabled = b;

        }
        else
        {
            enabled = b;
            //ACTIVATE CURRENT TURNHANDLER
            ActivateCorrectTurnHandler();

            InGameUIInstance.SetActive(true);
        }
        
    }
    /// <summary>
    /// sends event to gamebehaviour about pausing the game
    ///  and showing the main menu
    /// </summary>
    public void PressReturnToMenu()
    {
        OnReturnMenuButtonClick();
    }
}
