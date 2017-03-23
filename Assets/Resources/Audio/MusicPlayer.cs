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


//Music playlist with track-switching functionality that works independently of update-rate (in case you want to tinker with fixed update for slow-mo or stuff like that)

public class MusicPlayer : MonoBehaviour {

	public AudioClip[] audioClip;
	[HideInInspector]
	public AudioSource audioSource;

	[SerializeField]
	float fadeTime;

	public static MusicPlayer Instance = null;


	int nextTrackToPlay = 0;
	bool paused = false;
	bool repeat = false;
	bool switchTrack = false;
	bool firstUpdateSinceTrackSwitch = false;
	float fadeTimeRemaining = 0;
	float startVolume;

	void Awake(){
		if (Instance == null)
			Instance = this;
		else if (Instance != this) //Makes sure there's only one instance of the script
			Destroy(gameObject); //Goes nuclear
		DontDestroyOnLoad(gameObject);
		audioSource = GetComponent<AudioSource>();
		startVolume = audioSource.volume;
	}

	void Update(){
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

		if (switchTrack){
			if (firstUpdateSinceTrackSwitch){
				RepeatCheck ();
				firstUpdateSinceTrackSwitch = false;
			}
			fadeTimeRemaining -= Time.unscaledDeltaTime;
			audioSource.volume = Mathf.Lerp(fadeTime, 0, fadeTimeRemaining) * startVolume;
			if (fadeTimeRemaining < 0){
				audioSource.clip = audioClip[nextTrackToPlay];
				if (!paused)
					audioSource.Play();
				audioSource.volume = startVolume;
				fadeTimeRemaining = 0;
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
		if (audioClip.Length > 2){ //If only two files the other file is already selected automatically
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