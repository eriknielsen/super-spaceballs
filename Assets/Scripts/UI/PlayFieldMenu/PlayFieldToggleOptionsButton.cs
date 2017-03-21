using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayFieldToggleOptionsButton : MonoBehaviour {

	PlayFieldMenu menuHandler;

	void Awake(){
		GetComponent<Button>().colors = ToolBox.Instance.ButtonColors;
		menuHandler = FindObjectOfType<PlayFieldMenu>();
	}

	public void OnClick(){
		menuHandler.ToggleOptionsMenu();
	}
}