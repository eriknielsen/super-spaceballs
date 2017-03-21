using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	private Vector2 startPosition;

    [SerializeField]
    GameObject collideWithWallSound;

    Vector2 prevVelocity;
	Rigidbody2D rb2D;

    PreviewMarker previewMarker;
    LineRenderer localLineRenderer;
    public Vector2 PreviousVelocity
    {
        get { return prevVelocity; }
        set{ prevVelocity = value;}
    }
		
    void Start(){
		startPosition = transform.position;
		rb2D = GetComponent<Rigidbody2D>();
		localLineRenderer = GetComponent<LineRenderer>();
		previewMarker = GameObject.FindObjectOfType<PreviewMarker>().GetComponent<PreviewMarker>();
    }
 
    public void ResetPosition(){
		transform.position = startPosition;
        prevVelocity = Vector2.zero;
        rb2D.velocity = Vector2.zero;
        localLineRenderer.enabled = false;
        
	}

    public void DrawTrajectory(){
          if (prevVelocity.x != 0 && prevVelocity.y != 0){
            localLineRenderer.enabled = true;
			previewMarker.showBallDirection(transform.position, prevVelocity, localLineRenderer);
        } 
    }

    public void Pause(){
        prevVelocity = rb2D.velocity;
        rb2D.velocity = Vector2.zero;
       
        rb2D.freezeRotation = true;
        //if ball has a velocity, show it to the player
        DrawTrajectory();
    }

    public void Unpause(){
        rb2D.freezeRotation = false;
        rb2D.velocity = prevVelocity;

        localLineRenderer.enabled = false; 
    }

    void OnCollisionEnter2D(Collision2D other){
        if (other.collider.tag == "Wall"){
            AudioManager.Instance.PlayAudioWithRandomPitch(collideWithWallSound, false, gameObject);    
        }
        else if (other.collider.tag == "Robot"){
            AudioManager.Instance.PlayAudioWithRandomPitch(collideWithWallSound, false, gameObject);
        }
    }
}
