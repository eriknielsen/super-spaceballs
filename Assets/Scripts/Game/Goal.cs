using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Goal : MonoBehaviour {

	public int score { get; set; }

	public delegate void GoalScored();
	public static event GoalScored OnGoalScored;

	[SerializeField]
	bool left;

	Text scoreText;
	GameObject ball;
	GoalAnimScript goalAnimScript;
	float prevTimeScale;

	public GameObject longestCheer;

	void Start(){
		if (left)
			scoreText = GameObject.Find("RightScore").GetComponent<Text>(); //Opposite player gets increased score (obviously)
		else
			scoreText = GameObject.Find("LeftScore").GetComponent<Text>();
		score = 0;
		goalAnimScript = FindObjectOfType<GoalAnimScript>();

		ball = FindObjectOfType<Ball>().gameObject;
	}

	void OnTriggerEnter2D(Collider2D other){ //When ball enters goal ballposition is reset and score is increased
		if (other.gameObject == ball){
			GameObject.FindGameObjectWithTag("PlayController").GetComponent<IPlayBehaviour>().PreOnGoalScored();
			prevTimeScale = Time.timeScale;
			Time.timeScale = 0.5f;
			score++;
			scoreText.text = "" + score;
			AudioManager.Instance.PlayAudioWithRandomPitch(longestCheer, false, gameObject);

			goalAnimScript.GoalScored(left, this);
		}
	}

    public void Score(){
        ball.GetComponent<Ball>().ResetPosition();
        Time.timeScale = prevTimeScale;
        OnGoalScored();
    }
}