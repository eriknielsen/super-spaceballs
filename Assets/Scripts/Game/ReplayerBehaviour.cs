using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReplayerBehaviour : MonoBehaviour
{

    //####
    //private vars
    List<Move> movesToReplay;
    //#####
    //public vars

    // Use this for initialization
    void Start()
    {

        GameObject.Find("TurnHandler").SetActive(false);


        //GameObject.Find("TurnHandler").SetActive(false);




    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// should take a serialized file and load the moves into movesToReplay
    /// </summary>
    void LoadMovesFromFile()
    {

    }
    /// <summary>
    /// should take the moves from TurnHandler(or whichever class has them)
    /// and load them into movesToReplay
    /// </summary>
    void LoadMovesFromGame()
    {
        movesToReplay.AddRange(GameObject.Find("TurnHandler").
            GetComponent<TurnHandlerBehaviour>().moves);

    }
    /// <summary>
    /// should begin the game with robots at the starting positions
    /// found in the movesToReplay list in a paused state
    /// </summary>
    void InitReplay()
    {

    }
    /// <summary>
    /// Takes the correct moves and gives the commands to the robots
    /// and puts them into PlayState.
    /// </summary>
    void PlayTurn()
    {

    }
    static void SaveMovesToFile() { }

}
