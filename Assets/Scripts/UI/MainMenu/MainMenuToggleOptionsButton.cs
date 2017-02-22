using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuToggleOptionsButton : MonoBehaviour {

	private MainMenu menuHandler;

	void Awake(){
		menuHandler = GameObject.FindWithTag("MenuHandler").GetComponent<MainMenu>();
	}

	public void OnClick(){
		menuHandler.ToggleOptionsMenu();
	}
}