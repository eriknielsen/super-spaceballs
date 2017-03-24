using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayFieldButtons : MonoBehaviour {

	enum ButtonTypes { MainMenu, ToggleOptions, Quit };

	[SerializeField]
	ButtonTypes buttonType;

	private PlayFieldMenu menuHandler;

	void Start(){
		GetComponent<Button>().colors = ToolBox.Instance.ButtonColors;
		menuHandler = FindObjectOfType<PlayFieldMenu>();
	}

	public void OnClick(){
		if (buttonType == ButtonTypes.MainMenu){
			menuHandler.MainMenu();
		} else if (buttonType == ButtonTypes.ToggleOptions){
			menuHandler.ToggleOptionsMenu();
		} else if (buttonType == ButtonTypes.Quit){
			menuHandler.Quit();
		}
	}
}