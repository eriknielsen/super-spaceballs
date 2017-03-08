using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuToggleOptionsButton : MonoBehaviour {

	private MainMenu menuHandler;

	void Awake(){
		menuHandler = GameObject.FindWithTag("MenuHandler").GetComponent<MainMenu>();
		GetComponent<Button>().colors = ToolBox.Instance.buttonColors;
	}

	public void OnClick(){
		menuHandler.ToggleOptionsMenu();
	}
}