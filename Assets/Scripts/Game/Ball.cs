using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

	private Vector2 startPosition;
    //audio
    [SerializeField]
    GameObject collideWithWallSound;

	void Start(){
		startPosition = transform.position;
   
	}
	
	public void ResetPosition(){
		transform.position = startPosition;
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
