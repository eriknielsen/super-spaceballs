﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayFieldQuitButton : MonoBehaviour {

	private PlayFieldMenu menuHandler;

	void Awake(){
		menuHandler = GameObject.FindWithTag("MenuHandler").GetComponent<PlayFieldMenu>();
		GetComponent<Button>().colors = ToolBox.Instance.buttonColors;
	}

	public void OnClick(){
		menuHandler.Quit();
	}
}