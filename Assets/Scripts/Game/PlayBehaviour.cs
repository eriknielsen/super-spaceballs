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
            //decide if both turnhandlers have been activated yet or not.
        }
	}
    void NewTurn()
    {
        //count turns to decide who starts?
        //turnHandler1.activate();
        //turnhandler1activated= true
        //OR
        //turnHandler2.activate();
        //turnhandler2activated= true





    }
}
