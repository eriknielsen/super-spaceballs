using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainMenuLocalPlayButton : MonoBehaviour {

	MainMenu menuHandler;

	void Awake(){
		GetComponent<Button>().colors = ToolBox.Instance.ButtonColors;
		menuHandler = FindObjectOfType<MainMenu>();
	}

	public void OnClick(){
		menuHandler.LocalPlay();
	}
}