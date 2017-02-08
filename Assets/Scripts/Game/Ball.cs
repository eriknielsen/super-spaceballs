using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	private Vector2 startPosition;

	void Start(){
		startPosition = transform.position;
	}
	
	public void ResetPosition(){
		transform.position = startPosition;
	}
}
