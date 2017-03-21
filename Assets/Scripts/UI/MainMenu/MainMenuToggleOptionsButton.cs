using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuToggleOptionsButton : MonoBehaviour {

	private MainMenu menuHandler;

	void Awake(){
		GetComponent<Button>().colors = ToolBox.Instance.ButtonColors;
		menuHandler = FindObjectOfType<MainMenu>();
	}

	public void OnClick(){
		menuHandler.ToggleOptionsMenu();
	}
}