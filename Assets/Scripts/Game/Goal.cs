using UnityEngine;
using UnityEngine.UI;


public class Goal : MonoBehaviour {

	public int score { get; set; }
    public delegate void GoalScored();
    public static event GoalScored OnGoalScored;

	[SerializeField]
	bool Left;
    [SerializeField]

	public Text scoreText;

    Animator goalAnimator;
    GameObject ball;
    float prevTimeScale;

	void Start(){
		if (Left)
			scoreText = GameObject.Find("RightScore").GetComponent<Text>();
		else
			scoreText = GameObject.Find("LeftScore").GetComponent<Text>();
		score = 0;
        goalAnimator = gameObject.GetComponent<Animator>();
        if(goalAnimator != null)
        {
            Debug.Log("NOT NULL");
        }
        ball = FindObjectOfType<Ball>().gameObject;
    }

	void OnTriggerEnter2D(Collider2D other){ //When ball enters goal ballposition is reset and score is increased
        if (other.gameObject == ball){
            prevTimeScale = Time.timeScale;
            Time.timeScale = 0.5f;
            goalAnimator.SetTrigger("Score");
        }
	}

    void Score()
    {
        TurnHandlerBehaviour[] turnHandlers = FindObjectsOfType<TurnHandlerBehaviour>();
        foreach(TurnHandlerBehaviour turnhandler in turnHandlers)
        {
            turnhandler.Activate(false);
            Debug.Log(turnhandler.name);
        }
        Time.timeScale = prevTimeScale;

        OnGoalScored();
        ball.GetComponent<Ball>().ResetPosition();
        score++;
        scoreText.text = "" + score;
    }
}
