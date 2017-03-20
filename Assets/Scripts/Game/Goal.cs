using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour {

	public int score { get; set; }

	public delegate void GoalScored();
	public static event GoalScored OnGoalScored;


	[SerializeField]
	bool Left;

	Text scoreText;
	GameObject ball;
	Animator animator;
	float prevTimeScale;

	public GameObject longestCheer;

	void Start(){
		if (Left)
			scoreText = GameObject.Find("RightScore").GetComponent<Text>(); //Opposite player gets increased score (obviously)
		else
			scoreText = GameObject.Find("LeftScore").GetComponent<Text>();
		score = 0;
		animator = GetComponent<Animator>();
		ball = FindObjectOfType<Ball>().gameObject;
	}

	void OnTriggerEnter2D(Collider2D other){ //When ball enters goal ballposition is reset and score is increased
		if (other.gameObject == ball){
			GameObject.FindGameObjectWithTag("PlayController").GetComponent<IPlayBehaviour>().PreOnGoalScored();
			prevTimeScale = Time.timeScale;
			Time.timeScale = 0.5f;
			animator.SetTrigger("Score");
			AudioManager.instance.PlayAudioWithRandomPitch(longestCheer, false, gameObject);
		}
	}

	void Score(){
		Time.timeScale = prevTimeScale;
		ball.GetComponent<Ball>().ResetPosition();
		OnGoalScored();

		score++;
		scoreText.text = "" + score;
	}
}