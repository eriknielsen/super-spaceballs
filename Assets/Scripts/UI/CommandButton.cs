using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandButton : MonoBehaviour {

//	[SerializeField]
//	bool bounce = false;
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

//	bool animate = false;
//	bool firstUpdateSinceAnimationStart = false;
//	float lerp;
//	float timeSinceAnimationStart;
	Sprite normalSprite;
    IPlayBehaviour playBehaviour;

    void Start(){
		normalSprite = GetComponent<SpriteRenderer>().sprite;
		playBehaviour = GameObject.FindGameObjectWithTag("PlayController").GetComponent<IPlayBehaviour>();
    }

	void Update(){
//		if (animate) {
//			if (firstUpdateSinceAnimationStart) {
//				firstUpdateSinceAnimationStart = false;
//			}
//			if (bounce) {
//
//			} else {
//				timeSinceAnimationStart += Time.deltaTime;
//				lerp = Mathf.Lerp(0, animationTime, timeSinceAnimationStart);
//				transform.localScale = new Vector3(lerp, lerp, 1);
//			}
//		}
	}

	void OnMouseDown(){
		playBehaviour.SelectCommand(selectedCommand);
		if (selectedCommand == Command.AvailableCommands.None)
			commandWheelHandler.ToggleWithDelay(animationTime, animationTime);
		else
			commandWheelHandler.ToggleWithDelay(animationTime, animationTime);
		
		GetComponent<SpriteRenderer>().sprite = onMouseClickSprite;
//		animate = true;
//		firstUpdateSinceAnimationStart = true;
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