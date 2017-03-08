using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainMenuLocalPlayButton : MonoBehaviour {

	MainMenu menuHandler;

	void Awake(){
		menuHandler = GameObject.FindWithTag("MenuHandler").GetComponent<MainMenu>();
		GetComponent<Button>().colors = ToolBox.Instance.buttonColors;
	}

	public void OnClick(){
		menuHandler.LocalPlay();
	}
}