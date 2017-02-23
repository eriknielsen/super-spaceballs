using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandButton : MonoBehaviour {

	[SerializeField]
	private Command.AvailableCommands selectedCommand;
	[SerializeField]
	private float buttonAnimationDelay;

	void OnMouseDown(){
		PlayBehaviour.Instance.SelectedCommand(selectedCommand);
		transform.parent.parent.GetComponent<CommandWheelHandler>().SelectCommandUIChange(buttonAnimationDelay);
		//AnimateButtonClick
	}

//	void OnMouseEnter(){	//For animation
//		//Make brighter?
//	}
//
//	void OnMouseOver(){
//		//Do backflips?
//	}
//
//	void OnMouseExit(){
//		//Make normal
//	}
}