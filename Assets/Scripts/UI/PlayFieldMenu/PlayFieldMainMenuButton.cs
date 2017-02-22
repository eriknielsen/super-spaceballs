using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFieldMainMenuButton : MonoBehaviour {

	private PlayFieldMenu menuHandler;

	void Awake(){
		menuHandler = GameObject.FindWithTag("MenuHandler").GetComponent<PlayFieldMenu>();
	}

	public void OnClick(){
		menuHandler.MainMenu();
	}
}