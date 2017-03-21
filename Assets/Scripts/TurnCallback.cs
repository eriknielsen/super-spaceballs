using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCallback : MonoBehaviour {

	IPlayBehaviour playBehaviour;

	void Start(){
		playBehaviour = GameObject.FindGameObjectWithTag("PlayController").GetComponent<PlayBehaviour>();
	}

	void LeftTurn(){
		playBehaviour.LeftTurnAnimCallback();
	}

	void RightTurn(){
		playBehaviour.RightTurnAnimCallback();
	}
}
