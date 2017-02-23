using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	private Vector2 startPosition;
    //audio
    [SerializeField]
    GameObject collideWithWallSound;

    Vector2 prevVelocity;
    Rigidbody2D rb;
	void Start(){
		startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        PlayBehaviour.OnPauseGame += new PlayBehaviour.GamePaused(Pause);
        PlayBehaviour.OnUnpauseGame += new PlayBehaviour.GameUnpaused(Unpause);
    }
    public void ResetPosition(){
		transform.position = startPosition;
        Pause();
	}
    void Pause()
    {
        Debug.Log("pause in boll");
        prevVelocity = rb.velocity;
        rb.velocity = Vector2.zero;
        rb.freezeRotation = true;
    }
    void Unpause()
    {
        rb.freezeRotation = false;
        rb.velocity = prevVelocity;
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
