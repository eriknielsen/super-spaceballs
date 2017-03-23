using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainMenuLocalPlayButton : MonoBehaviour {

	MainMenu menuHandler;
	MainMenuMusic mainMenuMusic;

	void Awake(){
		GetComponent<Button>().colors = ToolBox.Instance.ButtonColors;
		menuHandler = FindObjectOfType<MainMenu>();
		mainMenuMusic = FindObjectOfType<MainMenuMusic>();
	}

	public void OnClick(){
		menuHandler.LocalPlay();
		//Show loading screen
	}
}