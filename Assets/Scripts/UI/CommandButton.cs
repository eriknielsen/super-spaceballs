using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandButton : MonoBehaviour {

	[SerializeField]
	private Command.AvailableCommands selectedCommand;
	[SerializeField]
	private float buttonAnimationDelay;

	private ToggleObjects commandWheelHandler;

	void Start(){
		commandWheelHandler = transform.parent.parent.GetComponent<ToggleObjects>();
	}

	void OnMouseDown(){
		PlayBehaviour.Instance.SelectedCommand(selectedCommand);
		if (selectedCommand == Command.AvailableCommands.None)
			commandWheelHandler.Toggle();
		else
			commandWheelHandler.ToggleWithDelay(buttonAnimationDelay, 0);
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