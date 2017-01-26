using UnityEngine;
using System.Collections;

public class PlayBehaviour : MonoBehaviour {

    //instanitate turnhandlers, one for each team
    GameObject turnHandlerPrefab;
    TurnHandlerBehaviour turnHandler1;
    TurnHandlerBehaviour turnHandler2;
    bool turnHandler1Activated = false;
    bool turnHandler2Activated = false;

	// Use this for initialization
	void Start () {
        //turn handlers start "deactivated"
        turnHandler1 = Instantiate(turnHandlerPrefab).GetComponent<TurnHandlerBehaviour>();
        turnHandler2 = Instantiate(turnHandlerPrefab).GetComponent<TurnHandlerBehaviour>();

    }

    // Update is called once per frame
    void Update () {
        //listen  for buttonpresses like wanting to send your move etc
        if (Input.GetKeyDown("Enter"))
        {
            // are both players done?
            if (turnHandler1Activated && turnHandler2Activated)
            {
                //then play the game and pause in 4 seconds
                //UnpauseGame();
            }
            else
            {
                //
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
        
    }
    void PauseGame()
    {
        Time.timeScale = 0;
        turnHandler1.PauseGame();
        turnHandler2.PauseGame();
        NewTurn();
    }
    void NextPlayer()
    {
        if (!turnHandler1Activated)
            //turnHandler1.activate();
            ;
        if (!turnHandler2Activated)
            //turnHandler2.activate();
            ;
    }
    void NewTurn()
    {
        if(turnHandler1.Turns % 2 == 0)
        {
            //turnHandler1.activate();
            turnHandler1Activated = true;
        }
        else
        {
            //turnHandler2.activate();
            turnHandler2Activated = true;
        }
    }
}
