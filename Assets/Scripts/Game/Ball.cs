using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	private Vector2 startPosition;
    //audio
    [SerializeField]
    GameObject collideWithWallSound;

    Vector2 prevVelocity;
    Rigidbody2D rb;
    static bool isSubscribing = false;

    public Vector2 PreviousVelocity
    {
        get { return prevVelocity; }
    }

	void Awake(){
		startPosition = transform.position;
        
        if (!isSubscribing)
        {
            PlayBehaviour.OnPauseGame += new PlayBehaviour.GamePaused(Pause);
            PlayBehaviour.OnUnpauseGame += new PlayBehaviour.GameUnpaused(Unpause);
            isSubscribing = true;
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }
    public void ResetPosition(){
		transform.position = startPosition;
        Pause();
	}
    void Pause()
    {
        prevVelocity = rb.velocity;
        rb.velocity = Vector2.zero;
        
        rb.freezeRotation = true;
    }

    void Unpause()
    {
        if(rb != null)
        {
            rb.freezeRotation = false;
            rb.velocity = prevVelocity;
        }
        else
        {
            Debug.Log("rb was null");
            rb = GetComponent<Rigidbody2D>();
            Debug.Log("rb is: " + rb);
            rb.freezeRotation = false;
            rb.velocity = prevVelocity;
        }
    
    }
    void OnDestroy()
    {
        PlayBehaviour.OnPauseGame -= Pause;
        PlayBehaviour.OnUnpauseGame -= Unpause;
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
