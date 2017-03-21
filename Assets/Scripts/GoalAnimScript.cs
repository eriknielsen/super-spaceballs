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

<<<<<<< HEAD
	void AfterGoalAnim(){ //Animation event called at end of animation
		goal.GetComponent<Goal>().ResetShit();
=======
	void AfterGoalAnim(){ //Animation event called at end of animation, same on both goals
		goal.ResetShit();
>>>>>>> a9aa4de33aa7f2f09397b2726a5c8e39dd94c4c3
	}
}
