using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainMenuButtons : MonoBehaviour {

	enum ButtonTypes { LocalPlay, OnlinePlay, ToggleOptions, Quit };

	[SerializeField]
	ButtonTypes buttonType;

	GameObject loadingScreen;
	MainMenu menuHandler;

	void Start(){
		GetComponent<Button>().colors = ToolBox.Instance.ButtonColors;
		menuHandler = FindObjectOfType<MainMenu>();
	}

	public void OnClick(){
		if (buttonType == ButtonTypes.LocalPlay){
			menuHandler.LocalPlay();
		} else if (buttonType == ButtonTypes.OnlinePlay){
			menuHandler.OnlinePlay();
		} else if (buttonType == ButtonTypes.ToggleOptions){
			menuHandler.ToggleOptionsMenu();
		} else if (buttonType == ButtonTypes.Quit){
			menuHandler.Quit();
		}
	}
}