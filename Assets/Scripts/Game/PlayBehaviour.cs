using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayBehaviour : MonoBehaviour
{
    //class for local play
    [SerializeField]
    TurnHandlerBehaviour leftPrefab;
    [SerializeField]
    TurnHandlerBehaviour rightPrefab;

    TurnHandlerBehaviour turnHandler1;
    TurnHandlerBehaviour turnHandler2;

    bool isTH1Done = false;
    bool isTH2Done = false;
    int currentTurnHandler;
    Vector2 score;
    GameTimer gameTimer;
    Collider2D[] collidersInGame;

    public Text gameTimeText;
    public GameObject InGameUIInstance;

    public float roundTime;
    public GameObject InGameUIPrefab;
    public GameObject turnHandlerPrefab;

    public delegate void ReturnMenuButtonClicked();
    public static event ReturnMenuButtonClicked OnReturnMenuButtonClick;

    public delegate void ReplayButtonClicked();
    public static event ReplayButtonClicked OnReplayButtonClick;

    public static PlayBehaviour instance;

    public static PlayBehaviour Instance
    {
        get { return instance; }
    }

    public static float RoundTime
    {
        get { return instance.roundTime; }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("There's already an instance of PlayBehaviour");
        }
        collidersInGame = FindObjectsOfType<Collider2D>();
        gameTimer = new GameTimer(120);
        InGameUIInstance = Instantiate(InGameUIPrefab);
        //CreateTurnHandlers();
        gameTimeText = InGameUIInstance.transform.FindChild("GameTimeText").GetComponent<Text>();
    }

    public void CreateTurnHandlers()
    {
        if (leftPrefab != null)
        {
            if (turnHandler1 == null)
            {
                turnHandler1 = Instantiate(leftPrefab);
                turnHandler1.transform.parent = transform;
            }
            else
            {
                Debug.LogWarning("'turnHandler1' already holds an instance of TurnHandlerBehaviour. Call 'DestroyTurnHandlers()' before calling this function.");
            }
        }
        else
        {
            Debug.LogWarning("'leftPrefab' doesn't hold a reference, so no TurnHandler is created.");
        }

        if (rightPrefab != null)
        {
            if (turnHandler2 == null)
            {
                turnHandler2 = Instantiate(rightPrefab);
                turnHandler2.transform.parent = transform;
            }
            else
            {
                Debug.LogWarning("'turnHandler2' already holds an instance of TurnHandlerBehaviour. Call 'DestroyTurnHandlers()' before calling this function.");
            }
        }
        else
        {
            Debug.LogWarning("'rightPrefab' doesn't hold a reference, so no TurnHandler is created.");
        }
    }

    void DestroyTurnHandlers()
    {
        if (turnHandler1 != null)
        {
            Destroy(turnHandler1.gameObject);
        }
        if (turnHandler2 != null)
        {
            Destroy(turnHandler2.gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        InGameUIInstance.SetActive(true);

        //decide who goes first
        //NewTurn();
        gameTimeText.text = "Time " + gameTimer.MinutesRemaining() + ":" + gameTimer.SecondsRemaining();


    }

    // Update is called once per frame
    void Update()
    {

        gameTimeText.text = "Time " + gameTimer.MinutesRemaining() + ":" + gameTimer.SecondsRemaining();

        //listen  for buttonpresses like wanting to send your move etc
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (currentTurnHandler == 1)
            {
                isTH1Done = true;
            }
            else if (currentTurnHandler == 2)
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
                ActivateTurnHandler(true);
            }
        }
    }
    ///if we replayed the last turn, then we dont want to do the newturn stuff
    IEnumerator UnpauseGame(bool asReplay)
    {
        TurnOnColliders();
        InGameUIInstance.transform.GetChild(0).gameObject.SetActive(false);
        InGameUIInstance.transform.GetChild(1).gameObject.SetActive(false);

        ActivateTurnHandler(false);
        if (asReplay)
        {
            turnHandler1.ReplayLastTurn();
            turnHandler2.ReplayLastTurn();
        }

        turnHandler1.UnpauseGame();
        turnHandler2.UnpauseGame();
        StartCoroutine(gameTimer.CountDownSeconds((int)roundTime));
        yield return new WaitForSeconds(roundTime);
        if (asReplay == false)
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
            ActivateTurnHandler(true);
        }
        PauseGame();


    }
    void PauseGame()
    {
        InGameUIInstance.transform.GetChild(0).gameObject.SetActive(true);
        InGameUIInstance.transform.GetChild(1).gameObject.SetActive(true);

        turnHandler1.PauseGame();
        turnHandler2.PauseGame();
        TurnOffColliders();
    }

    void TurnOffColliders()
    {
        collidersInGame = FindObjectsOfType<Collider2D>();
        string exception = "Clickable Hitbox";
        for (int i = 0; i < collidersInGame.Length; i++)
        {
            if (collidersInGame[i].tag != exception)
            {
                collidersInGame[i].enabled = false;
            }
        }
    }

    void TurnOnColliders()
    {
        collidersInGame = FindObjectsOfType<Collider2D>();
        for (int i = 0; i < collidersInGame.Length; i++)
        {
            collidersInGame[i].enabled = true;
        }
    }

    /// <summary>
    /// gives or takes control from the current turnhandler
    /// activate true means to activate current turnhandler
    /// activate false means to deactivate current turnhandler
    /// </summary>
    void ActivateTurnHandler(bool activate)
    {
        //TurnOnColliders();
        if (!turnHandler1.gameObject.activeSelf)
        {
            turnHandler1.gameObject.SetActive(true);
        }
        if (!turnHandler2.gameObject.activeSelf)
        {
            turnHandler2.gameObject.SetActive(true);
        }


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
            turnHandler1.Activate(false);
            turnHandler2.Activate(false);
        }
    }

    void NewTurn()
    {
        if (turnHandler1.Turns % 2 == 0)
        {
            currentTurnHandler = 1;

        }
        else
        {
            currentTurnHandler = 2;

        }
        isTH1Done = false;
        isTH2Done = false;

        ActivateTurnHandler(true);
    }
    void ChooseNextCurrentTurnHandler()
    {
        //if current is one and finished AND other one isnt done, 
        //give control to the other one
        if (currentTurnHandler == 1 && isTH1Done && isTH2Done == false)
        {
            currentTurnHandler = 2;
        }
        else if (currentTurnHandler == 2 && isTH2Done && isTH1Done == false)
        {
            currentTurnHandler = 1;
        }
        else if (isTH1Done && isTH2Done)
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
        if (activate == false)
        {
            DestroyTurnHandlers();
            InGameUIInstance.SetActive(false);
            enabled = false;
        }
        else
        {
            enabled = true;
            CreateTurnHandlers();
            NewTurn();
            InGameUIInstance.SetActive(true);
            TurnOffColliders();
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
        if (turnHandler1.Turns > 1)
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
