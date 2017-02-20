using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InWorldCommandButton : MonoBehaviour {

	[SerializeField]
	private string commandName;

	void OnMouseDown(){
		switch (commandName){
		case "Move":
			//Do move command
			break;
		case "Push":
			//Do push command
			break;
		}
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