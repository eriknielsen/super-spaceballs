using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Goal : MonoBehaviour {

	public int score { get; set; }

	public Text scoreText;

	void Start(){
		score = 0;
		scoreText.text = "" + score;
	}

	void OnTriggerEnter2D(Collider2D other){ //When ball enters goal ballposition is reset and score is increased
		if (other.tag == "Ball"){
			other.gameObject.GetComponent<Ball>().ResetPosition();
			ModifyScore(1);
		}
	}

	public void ModifyScore(int scoreModifier){
		score += scoreModifier;
		scoreText.text = "" + score;
	}
}
