using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalAnimScript : MonoBehaviour {

	Goal goal;
	Animator animator;

	void Start(){
		animator = GetComponent<Animator>();
	}

	public void GoalScored(bool left, Goal goalE){
		goal = goalE;
		if (left)
			animator.SetTrigger("GoalOnLeft");
		else
			animator.SetTrigger("GoalOnRight");
	}

	void AfterGoalAnim(){ //Animation event called at end of animation, same on both goals
		goal.ResetShit();
	}
}
