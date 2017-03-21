using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayFieldQuitButton : MonoBehaviour {

	private PlayFieldMenu menuHandler;

	void Start(){
		GetComponent<Button>().colors = ToolBox.Instance.ButtonColors;
		menuHandler = FindObjectOfType<PlayFieldMenu>();
	}

	public void OnClick(){
		menuHandler.Quit();
	}
}