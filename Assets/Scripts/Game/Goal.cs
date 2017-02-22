using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Goal : MonoBehaviour {

	public int score { get; set; }
    public delegate void GoalScored();
    public event GoalScored OnGoalScored;

	void Start(){
		score = 0;
	}

	void OnTriggerEnter2D(Collider2D other){ //When ball enters goal ballposition is reset and score is increased
		if (other.tag == "Ball"){
            Debug.Log("score! this goal's team has points: " + score);
            OnGoalScored();
			other.gameObject.GetComponent<Ball>().ResetPosition();
		}
	}
}
