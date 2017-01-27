using UnityEngine;
using System.Collections;

public class PlayBehaviour : MonoBehaviour {
    //class for local play

  
    
    
    GameObject InGameUIInstance;
    TurnHandlerBehaviour turnHandler1;
    TurnHandlerBehaviour turnHandler2;
    bool isTH1Done = false;
    bool isTH2Done = false;
    int currentTurnHandler;

    public GameObject InGameUIPrefab;
    public GameObject turnHandlerPrefab;

    public delegate void ReturnMenuButtonClicked();
    public static event ReturnMenuButtonClicked OnReturnMenuButtonClick;

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
                //then play the game and pause again in 4 seconds
                StartCoroutine(UnpauseGame());
            }
            //decides who plays next if both players arent done
            else
            {
                GiveControlToNextPlayer();
            }
            
            
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
    /// gives or takes control from the current turnhandler
    /// activate true means to activate current turnhandler
    /// activate false means to deactivate current turnhandler
    /// </summary>
    void ActivateCorrectTurnHandler(bool activate)
    {
        if (activate)
        {
            if (currentTurnHandler == 1 && !isTH1Done)
            {
                turnHandler1.Activate(true);
            }
            else if (currentTurnHandler == 2 && !isTH2Done)
            {
                turnHandler2.Activate(true);
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
    /// <summary>
    /// this either pauses the game and deactivates ui and active turnhandler
    ///  or it activates ui and active turnhandler
    ///  Is called by GameBehaviour
    /// </summary>
    /// <param name="b"></param>
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
            enabled = false;
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
        OnReturnMenuButtonClick();
    }
}
