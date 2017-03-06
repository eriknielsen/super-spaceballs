using UnityEngine;
using System.Collections;

//Music playlist that works independently of update-rate (in case you want to tinker with fixed update for slow-mo or stuff like that)

public class SpaceballsRadio : MonoBehaviour {

	public AudioClip[] audioClip;
	[HideInInspector]
	public AudioSource audioSource;

	public static SpaceballsRadio instance = null;

	void Awake(){
		if (instance == null)
			instance = this;
		else if (instance != this){ //Makes sure there's only one instance of the script
			Destroy(gameObject); //Goes nuclear
		}
		DontDestroyOnLoad(gameObject);
	}


	int nextTrackToPlay = 0;
	bool paused = false;
	bool repeat = false;
	bool switchTrack = false;
	bool fadeStarted = false;
	bool firstUpdateSinceTrackSwitch = false;
	[SerializeField]
	float fadeTime;
	float timeSinceFadeStart = 0;
	float startVolume;

	void Start(){
		audioSource = GetComponent<AudioSource>();
		startVolume = audioSource.volume;
	}

	void Update(){
		if (switchTrack){
			if (firstUpdateSinceTrackSwitch){
				RepeatCheck ();
				firstUpdateSinceTrackSwitch = false;
			}
			timeSinceFadeStart += Time.deltaTime;
			audioSource.volume = Mathf.Lerp(fadeTime, 0, timeSinceFadeStart) * startVolume;
			if (timeSinceFadeStart > fadeTime){
				audioSource.clip = audioClip[nextTrackToPlay];
				if (!paused)
					audioSource.Play();
				audioSource.volume = startVolume;
				timeSinceFadeStart = 0;
				switchTrack = false;
			}
		}
		else if (!paused){
			if (!audioSource.isPlaying){
				audioSource.clip = audioClip[nextTrackToPlay];
				audioSource.Play();
				RepeatCheck();
			}
		}
	}

	void RepeatCheck(){
		if (!repeat){
			nextTrackToPlay++;
			if (nextTrackToPlay > audioClip.Length - 1)
				nextTrackToPlay = 0;
		}
	}

	public void PlayPause(){
		paused = !paused;
		if (paused)
			audioSource.Pause();
		else {
			audioSource.UnPause();
			if (!audioSource.isPlaying)
				audioSource.Play();
		}
	}

	public void NextTrack(){
		firstUpdateSinceTrackSwitch = true;
		switchTrack = true;
	}

	public void PreviousTrack(){
		if (audioClip.Length > 2){ //If only two files the other file is already selected automatically in RepeatCheck
			nextTrackToPlay -= 2;
			if (nextTrackToPlay < 0){
				if (nextTrackToPlay < -1)
					nextTrackToPlay = audioClip.Length - 2;
				else
					nextTrackToPlay = audioClip.Length - 1;
			}
		}
		firstUpdateSinceTrackSwitch = true;
		switchTrack = true;
	}

	public void Repeat(){
		repeat = !repeat;
		if (!repeat && audioSource.clip == audioClip[nextTrackToPlay]){
			nextTrackToPlay++;
			if (nextTrackToPlay > audioClip.Length - 1)
				nextTrackToPlay = 0;
		}
	}

	public void Stop(){
		paused = true;
		audioSource.Stop();
	}
}