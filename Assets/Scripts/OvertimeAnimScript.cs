using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvertimeAnimScript : MonoBehaviour {

	IPlayBehaviour playBehaviour;
	Animator animator;

	void Start(){
		animator = GetComponent<Animator>();
	}

	public void StartAnimation(IPlayBehaviour pb){
		animator.SetTrigger("Overtime");
		playBehaviour = pb;
	}

	void AfterAnimationEvent(){ //Animation event called at end of animation
		playBehaviour.OvertimeAnimCallback();
	}
}