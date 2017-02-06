using UnityEngine;
using System.Collections;

public class SoundTest : MonoBehaviour {

	public GameObject testSound;
	public GameObject testSound2;

	void Start () {
		//AudioManager.instance.PlayAudio(testSound, true, this.gameObject);
	}
	
	void FixedUpdate () {
		//SoundManagerAPI.instance.PlaySFX(testSound, true, this.gameObject);
	}

	public void PlayHorsieSound(){
		AudioManager.instance.PlayAudio(testSound, true, this.gameObject);
	}
	public void PlayBaseSound(){
		AudioManager.instance.PlayAudio(testSound2, true, this.gameObject);
	}
}
