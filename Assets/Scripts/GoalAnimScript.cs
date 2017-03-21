using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalAnimScript : MonoBehaviour {

	GameObject goal;
	Animator animator;

	void Start(){
		animator = GetComponent<Animator>();
	}

	public void GoalScored(bool left, GameObject goalE){
		goal = goalE;
		if (left)
			animator.SetTrigger("GoalOnLeft");
		else
			animator.SetTrigger("GoalOnRight");
	}

	void AfterGoalAnim(){ //Animation event called at end of animation, same on both goals
		goal.GetComponent<Goal>().ResetShit();
	}
}
