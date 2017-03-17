using UnityEngine;
using UnityEngine.UI;


public class Goal : MonoBehaviour {

	public int score { get; set; }
    public delegate void GoalScored();
    public static event GoalScored OnGoalScored;

	[SerializeField]
	bool Left;

	Text scoreText;

	void Start(){
		if (Left)
			scoreText = GameObject.Find("RightScore").GetComponent<Text>();
		else
			scoreText = GameObject.Find("LeftScore").GetComponent<Text>();
		score = 0;
	}

	void OnTriggerEnter2D(Collider2D other){ //When ball enters goal ballposition is reset and score is increased
		if (other.tag == "Ball"){
			other.gameObject.GetComponent<Ball>().ResetPosition();
			OnGoalScored();
			score++;
            scoreText.text = "" + score;
		}
	}
}
