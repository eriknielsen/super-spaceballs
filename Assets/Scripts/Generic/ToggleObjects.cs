using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObjects : MonoBehaviour {

	[SerializeField]
	private GameObject object1;
	[SerializeField]
	private GameObject object2;

	public void ToggleWithDelay(float delay1, float delay2){
		Invoke ("Toggle1", delay1); //FixedUpdate time problem? Divide by timescale?
		Invoke ("Toggle2", delay2);
	}

	public void Toggle(){
		Toggle1();
		Toggle2();
	}

	public void Toggle1(){
		object1.SetActive(!object1.activeInHierarchy);
	}

	public void Toggle2(){
		object2.SetActive(!object2.activeInHierarchy);
	}
}
