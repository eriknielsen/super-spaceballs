using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotDeselectionCollider : MonoBehaviour {

	void OnMouseDown(){
		if (Input.GetMouseButtonDown(0))
			GameObject.Find("LocalPlay").GetComponent<PlayBehaviour>().DeselectRobot();
	}
}