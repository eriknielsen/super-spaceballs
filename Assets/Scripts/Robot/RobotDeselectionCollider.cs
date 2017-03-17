using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotDeselectionCollider : MonoBehaviour {

    IPlayBehaviour pb;

    void Start(){
        pb = GameObject.FindGameObjectWithTag("PlayController").GetComponent<IPlayBehaviour>();
    }

	void OnMouseDown(){
        if (Input.GetMouseButtonDown(0))
            pb.DeselectRobot();
	}
}