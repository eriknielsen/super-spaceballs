using UnityEngine;
using System.Collections;

public class SoundTest : MonoBehaviour {

	public GameObject testSound;
	public GameObject testSound2;

	void Start () {
		SoundManagerAPI.instance.PlaySFX(testSound, true, this.gameObject);
		//SoundManagerAPI.instance.PlayMusic(testSound2, true, this.gameObject);
	}
	
	void FixedUpdate () {
		//SoundManagerAPI.instance.PlaySFX(testSound, true, this.gameObject);
	}
}
