using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandButton : MonoBehaviour {

	[SerializeField]
	float animationTime;
	[SerializeField]
	Command.AvailableCommands selectedCommand;
	[SerializeField]
	ToggleObjects commandWheelHandler;
	[SerializeField]
	Sprite onMouseEnterSprite;
	[SerializeField]
	Sprite onMouseClickSprite;
    
	bool clicked = false;
	Sprite normalSprite;
    IPlayBehaviour playBehaviour;

    void Start(){
		normalSprite = GetComponent<SpriteRenderer>().sprite;
		playBehaviour = GameObject.FindGameObjectWithTag("PlayController").GetComponent<IPlayBehaviour>();
    }

	void OnMouseDown(){
		if (!clicked){
			playBehaviour.SelectCommand(selectedCommand);
			if (selectedCommand == Command.AvailableCommands.None)
				commandWheelHandler.ToggleWithDelay(animationTime, animationTime);
			else
				commandWheelHandler.ToggleWithDelay(animationTime, animationTime);
		
			GetComponent<SpriteRenderer>().sprite = onMouseClickSprite;
		}
	}

	void OnMouseEnter(){	//For animation
		GetComponent<SpriteRenderer>().sprite = onMouseEnterSprite;
	}

	void OnMouseOver(){
		//Do backflips?
	}

	void OnMouseExit(){
		GetComponent<SpriteRenderer>().sprite = normalSprite;
	}
}