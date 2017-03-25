using UnityEngine;
using System.Collections;
//using System.Collections.Generic; //Incomplete
//using System.IO;
//
//public class SoundPlayer : MonoBehaviour {
//
//	string absolutePath = "./"; // relative path to where the app is running
//	string[] fileTypes = {"ogg"}; //compatible file extensions
//
//	int soundIndex = 0;
//	FileInfo[] files;
//	AudioSource audioSource;
//	List<AudioClip> clips = new List<AudioClip>();
//
//	void Start(){
//		if(Application.isEditor) //being able to test in unity
//			absolutePath = "Assets/";
//		if(audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
//			reloadSounds();
//	}
//
//	void reloadSounds(){
//		DirectoryInfo info = new DirectoryInfo(absolutePath);
//		files = info.GetFiles();
//
//
//		foreach (FileInfo f in files){ //check if the file is valid and load it
//			if (validFileType(f.FullName)){
//				StartCoroutine(loadFile(f.FullName));
//			}
//		}
//	}
//
//	bool validFileType(string filename){
//		foreach (string ext in fileTypes){
//			if (filename.IndexOf(ext) > -1)
//				return true;
//		}
//		return false;
//	}
//
//	IEnumerator loadFile(string path){
//		WWW www = new WWW("file://"+path);
//
//		AudioClip myAudioClip = www.audioClip;
//		while (!myAudioClip.isReadyToPlay)
//			yield return www;
//
//		AudioClip clip = www.GetAudioClip(false);
//		string[] parts = path.Split('\\');
//		clip.name = parts[parts.Length - 1];
//		clips.Add(clip);
//	}
//}

//Music playlist with track-switching functionality that works independently of update-rate (in case you want to tinker with fixed update for slow-mo or stuff like that) if unscaledTime is set to true

public class MusicPlayer : MonoBehaviour {

	public bool unscaledTime = false; //Makes sure shit works on modified timescales
	public AudioClip[] audioClip;
	[HideInInspector]
	public AudioSource audioSource;

	[SerializeField]
	float fadeTime = 0.5f;

	public static MusicPlayer Instance = null;


	int nextTrackToPlay = 0; //Index
	[HideInInspector]
	public bool paused = false;
	bool repeat = false;
	float startVolume;
	Coroutine switchTrackAfterFade = null;
	Coroutine trackDuration = null;

	void Awake(){
		if (Instance == null)
			Instance = this;
		else if (Instance != this){ //Makes sure there's only one instance of the script
			Destroy(gameObject); //Goes nuclear
			return;
		}
		DontDestroyOnLoad(gameObject);
		paused = false;
		audioSource = GetComponent<AudioSource>();
		startVolume = audioSource.volume;
		SwitchTrack(); //Starts playing the first track
	}

	void Update(){ //Input only
		if (Input.GetKeyDown(KeyCode.RightArrow))
			NextTrack();
		if (Input.GetKeyDown(KeyCode.LeftArrow))
			PreviousTrack();
		if (Input.GetKeyDown(KeyCode.UpArrow))
			Stop();
		if (Input.GetKeyDown(KeyCode.DownArrow))
			PlayPause();
		if (Input.GetKeyDown(KeyCode.R))
			Repeat();
	}

	IEnumerator SwitchTrackAfterFade(float fadeTime){
		StartCoroutine(AudioManager.Instance.FadeSound(audioSource, fadeTime));

		if (unscaledTime)
			yield return new WaitForSecondsRealtime(fadeTime);
		else
			yield return new WaitForSeconds(fadeTime);

		SwitchTrack();

		audioSource.volume = startVolume;
		switchTrackAfterFade = null;
	}

	IEnumerator AutomaticTrackSwitch(float clipDuration){
		if (unscaledTime)
			yield return new WaitForSecondsRealtime(clipDuration);
		else
			yield return new WaitForSeconds(clipDuration);

		SwitchTrack();
		trackDuration = StartCoroutine(AutomaticTrackSwitch(audioSource.clip.length)); //Needs to use the length of the track switched to
	}

	void SwitchTrack(){
		audioSource.clip = audioClip[nextTrackToPlay];
		if (!paused){
			audioSource.Play(); //Needs to start again since it automatically stops when switching clips
			if (trackDuration != null)
				StopCoroutine(trackDuration);
			trackDuration = StartCoroutine(AutomaticTrackSwitch(audioSource.clip.length));
		}
		if (!repeat)
			IterateNextTrack();
	}

	void IterateNextTrack(){
		nextTrackToPlay++;
		if (nextTrackToPlay > audioClip.Length - 1)
			nextTrackToPlay = 0;
	}

	void DecrementNextTrack(){
		nextTrackToPlay--;
		if (nextTrackToPlay < 0)
			nextTrackToPlay = audioClip.Length - 1;
	}

	public void PlayPause(){
		paused = !paused;
		if (paused){
			audioSource.Pause();
			StopCoroutine(trackDuration);
		} else {
			audioSource.Play();
			trackDuration = StartCoroutine(AutomaticTrackSwitch(audioSource.clip.length - audioSource.time));
		}
	}

	public void NextTrack(){
		if (switchTrackAfterFade == null)
			switchTrackAfterFade = StartCoroutine(SwitchTrackAfterFade(fadeTime));
		else
			IterateNextTrack();
	}

	public void PreviousTrack(){
		if (repeat){
			DecrementNextTrack();
		}
		if (switchTrackAfterFade == null){
			switchTrackAfterFade = StartCoroutine(SwitchTrackAfterFade(fadeTime));
				
			if (audioClip.Length > 2){ //If only two files the other file is already selected automatically
				nextTrackToPlay -= 2;
				if (nextTrackToPlay < 0){
					if (nextTrackToPlay < -1)
						nextTrackToPlay = audioClip.Length - 2;
					else
						nextTrackToPlay = audioClip.Length - 1;
				}
			}
		} else
			DecrementNextTrack();
	}

	public void Repeat(){
		repeat = !repeat;
		if (!repeat && audioSource.clip == audioClip[nextTrackToPlay]){
			IterateNextTrack();
		}
	}

	public void Stop(){
		paused = true;
		if (trackDuration != null)
			StopCoroutine(trackDuration);
		audioSource.Stop();
	}

	void OnDestroy(){
		StopAllCoroutines();
	}
}