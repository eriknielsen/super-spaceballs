using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotDeselectionCollider : MonoBehaviour {

    IPlayBehaviour playBehaviour;

    void Start(){
		playBehaviour = GameObject.FindGameObjectWithTag("PlayController").GetComponent<IPlayBehaviour>();
    }

	void OnMouseDown(){
        if (Input.GetMouseButtonDown(0))
			playBehaviour.DeselectRobot();
	}
}