using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandWheelHandler : MonoBehaviour {

	[SerializeField]
	private GameObject commandButtons;
	[SerializeField]
	private GameObject backButton;

	public void SelectCommandUIChange(float delay){  //Hides and shows relevant buttons
		Invoke ("ToggleButtonSet1", delay); //FixedUpdate time problem? Divide by Time.fixedDeltaTime?
		ToggleButtonSet2();
	}

	public void DeselectCommandUIChange(){
		ToggleButtonSet1();
		ToggleButtonSet2();
	}

	private void ToggleButtonSet1(){
		commandButtons.SetActive(!commandButtons.activeInHierarchy);
	}

	private void ToggleButtonSet2(){
		backButton.SetActive(!backButton.activeInHierarchy);
	}
}
