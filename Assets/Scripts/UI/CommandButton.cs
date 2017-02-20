using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandButton : MonoBehaviour {

	[SerializeField]
	private Command.AvailableCommands selectedCommand;

	void OnMouseDown(){
//		TurnHandlerBehaviour.SetSelectedCommand(selectedCommand); //NEEDS PUBLIC ACCESS
	}

	//For animation
	void OnMouseEnter(){
		//Make brighter?
	}

	void OnMouseOver(){
		//Do backflips?
	}

	void OnMouseExit(){
		//Make normal
	}
}