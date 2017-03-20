using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
		animator = GameObject.Find("GoalAnimator").GetComponent<Animator>();

        ball = FindObjectOfType<Ball>().gameObject;
	}

	void OnTriggerEnter2D(Collider2D other){ //When ball enters goal ballposition is reset and score is increased
		if (other.gameObject == ball){
			GameObject.FindGameObjectWithTag("PlayController").GetComponent<IPlayBehaviour>().PreOnGoalScored();
            StartCoroutine(GoalEvent());
		}
	}

    IEnumerator GoalEvent()
    {
        prevTimeScale = Time.timeScale;
        Time.timeScale = 0.5f;

        if (Left)
        {
            animator.SetTrigger("ScoreOnLeft");
        }
        else
        {
            animator.SetTrigger("ScoreOnRight");
        }
        AudioManager.instance.PlayAudioWithRandomPitch(longestCheer, false, gameObject);
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("End State"))
        {
            yield return new WaitForSeconds(0.00001f);
        }
        animator.SetTrigger("ResetState");
        Time.timeScale = prevTimeScale;
        Score();
    }

    void Score(){
		ball.GetComponent<Ball>().ResetPosition();
		OnGoalScored();
        score++;
		scoreText.text = "" + score;
	}
}