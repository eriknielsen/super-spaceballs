using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	private Vector2 startPosition;
    //audio
    [SerializeField]
    GameObject collideWithWallSound;

    Vector2 prevVelocity;
    Rigidbody2D rb;

    PreviewMarker pm;
    LineRenderer localLineRenderer;
    public Vector2 PreviousVelocity
    {
        get { return prevVelocity; }
    }

	void Awake(){
		startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        localLineRenderer = GetComponent<LineRenderer>();
    }
    void Start()
    {
        pm = GameObject.Find("PreviewMarker").GetComponent<PreviewMarker>();

    }
 
    public void ResetPosition(){
		transform.position = startPosition;
        prevVelocity = Vector2.zero;
        rb.velocity = Vector2.zero;
        localLineRenderer.enabled = false;
        
	}
    public void Pause()
    {
        prevVelocity = rb.velocity;
        rb.velocity = Vector2.zero;
       
        rb.freezeRotation = true;
        //if ball has a velocity, show it to the player
        if(prevVelocity.x != 0 && prevVelocity.y != 0)
        {
            localLineRenderer.enabled = true;
            pm.showBallDirection(transform.position, prevVelocity, localLineRenderer);
        }    
    }
    public void Unpause()
    {
        rb.freezeRotation = false;
        rb.velocity = prevVelocity;

        localLineRenderer.enabled = false; 
    

    }
    void OnCollisionEnter2D(Collision2D other)
    {

        if (other.collider.tag == "Wall")
        {
            AudioManager.instance.PlayAudioWithRandomPitch(
                collideWithWallSound, false, gameObject);    
        }
        else if(other.collider.tag == "Robot")
        {
            AudioManager.instance.PlayAudioWithRandomPitch(
               collideWithWallSound, false, gameObject);
        }
    }
}
