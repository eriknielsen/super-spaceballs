using UnityEngine;
using System.Collections;

public class FadingSound : MonoBehaviour {

	public float fadeTarget; //Multiple to fade volume by (0.8 fades volume by 20%)
	public float fadeDelay; //How long after sound starts playing the fade starts
	public float fadeDuration; //Over how long the fade takes place
	public float updatesPerSecond; //Number of times per second that the volume is interpolated. 100 or more is recommended

	private float startVolume;
	private float startTime;
	private AudioSource sound;

	void Start () {
		sound = GetComponent<AudioSource>();
		startTime = Time.time;
		startVolume = sound.volume;
		StartCoroutine("FadeCoroutine");
	}

	void FixedUpdate () {
		
	}
	
	IEnumerator FadeCoroutine() {
		float fadeTime = 0;
		while (Time.time < startTime + fadeDelay + fadeDuration) {
			if (Time.time > startTime + fadeDelay) {
				fadeTime = (Time.time - (startTime + fadeDelay)) / fadeDuration;
				Debug.Log (fadeTime);
				sound.volume = FadeLERP(startVolume, fadeTarget, fadeTime);
			}
			yield return new WaitForSeconds(1 / updatesPerSecond);
		}
	}

	private float FadeLERP(float startValue, float fadeTarget, float currentTime){
		float lerpedFloat = Mathf.Lerp(startValue, fadeTarget, currentTime);
		return lerpedFloat;
	}
}
