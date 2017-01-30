using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public AudioSource horseSound;

	void Start () {
		SoundManager.instance.PlaySFX(horseSound);
		SoundManager.instance.ResetVolume ();
		Debug.Log(SoundManager.instance.globalVolume);
	}
	
	void FixedUpdate () {
		SoundManager.instance.ResetVolume ();
	}
}
