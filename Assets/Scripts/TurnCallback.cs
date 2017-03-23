using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCallback : MonoBehaviour {

	IPlayBehaviour playBehaviour;
	
	void Awake(){
        playBehaviour = FindObjectOfType<PlayBehaviour>();
        if(playBehaviour == null)
        {
            playBehaviour = FindObjectOfType<NetworkPlayBehaviour>();
        }
    }

	void LeftTurn(){
		playBehaviour.LeftTurnAnimCallback();
	}

	void RightTurn(){
		playBehaviour.RightTurnAnimCallback();
	}
}
