using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour {

	IPlayBehaviour playBehaviour;

	void Start(){
		GetComponent<Button>().colors = ToolBox.Instance.ButtonColors;
		playBehaviour = GameObject.FindGameObjectWithTag("PlayController").GetComponent<IPlayBehaviour>();
	}

	public void OnClick(){
		playBehaviour.EndTurn();
	}
}