using UnityEngine;
using System.Collections;

// This is a singleton that will act as an interface between the implementation of sound-handling and the scripts requesting audio to be played.
// Instructions:
// Add a reference to the game object prefab that contains the audio you want to play: public GameObject SoundObjectVariableNameHere;
// Which obviously means you need an audio prefab. You can create one by simply dragging an audio file into the Hierarchy, dragging the resulting object into the prefabs/sounds folder and deleting the object in the hierarchy)
// Call the function you want to use: ArsonisticSoundManager.instance.PlaySFX(SoundObjectVariableNameHere, false, this.gameObject);
// Where "SoundObjectVariableNameHere" is a variable name of your choice (just needs to match variable declaration), "false" is whether the sound will follow the object emitting the sound (set to true in case of for example a car engine sound following the car),
// and "this.gameObject" SHOULD NOT BE TOUCHED. It's a reference to the game object containing the script; the sound manager needs to know what object is creating the sound to be able to localize it.
// Changing volume is done through simply assigning the Global/Music/SFX volume variables a value between 1 and 0. Note that setting global and music volume to 50% results in an effective music volume of 25% since it is halved twice.
// Example: ArsonisticSoundManager.instance.GlobalVolume = 0.5;

public class ArsonisticSoundManager : MonoBehaviour {

	public static ArsonisticSoundManager instance = null;
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this) { //Makes sure there's only one instance of the script
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
		ResetVolume();
    }

    [Range(0, 1)]
    public float defaultGlobalVolume;
    [Range(0, 1)]
    public float defaultMusicVolume;
    [Range(0, 1)]
    public float defaultSFXVolume;
    public float minimumTimeBetweenSFX;
    public float highPitchRange = 1.05f;
    public float lowPitchRange = 0.95f;

	private float globalVolume; //Never access these directly
	private float musicVolume;
	private float sFXVolume;
	public float GlobalVolume { //Handles volume management for sound that is already playing
		get	{return globalVolume;}
		set	{
			globalVolume = value;
			for (int i = 0; i < SoundList.Count; i++) {
				if (SoundList[i].GetComponent<SoundManagementAddOn>().SoundType == "Music")
					AdjustPlayingMusicVolume (i);
				else if (SoundList[i].GetComponent<SoundManagementAddOn>().SoundType == "SFX")
					AdjustPlayingSFXVolume (i);
			}
		}
	}
	public float MusicVolume {
		get	{return musicVolume;}
		set	{
			musicVolume = value;
			for (int i = 0; i < SoundList.Count; i++) {
				if (SoundList[i].GetComponent<SoundManagementAddOn>().SoundType == "Music")
					AdjustPlayingMusicVolume (i);
			}
		}
	}
	public float SFXVolume {
		get	{return sFXVolume;}
		set	{
			sFXVolume = value;
			for (int i = 0; i < SoundList.Count; i++) {
				if (SoundList[i].GetComponent<SoundManagementAddOn>().SoundType == "SFX")
					AdjustPlayingSFXVolume(i);
			}
		}
	}

	private float currentClipVolume;
	public System.Collections.Generic.List<GameObject> SoundList;

	void Start(){
		SoundList = new System.Collections.Generic.List<GameObject>();
	}

    public void ResetVolume() {
		GlobalVolume = defaultGlobalVolume;
		MusicVolume = defaultMusicVolume;
		SFXVolume = defaultSFXVolume;
    }

	public void PlaySFX(GameObject sound, bool follow, GameObject callingObject) {
		currentClipVolume = SFXVolume * GlobalVolume * sound.GetComponent<AudioSource>().volume;
		PlaySound(sound, follow, currentClipVolume, "SFX", 1f, callingObject.transform);
    }

	public void PlayReptitiveSFX(GameObject sound, bool follow, GameObject callingObject) {
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
		currentClipVolume = SFXVolume * GlobalVolume * sound.GetComponent<AudioSource>().volume;
		PlaySound(sound, follow, currentClipVolume, "SFX", randomPitch, callingObject.transform);
    }

	public void PlayMusic(GameObject sound, bool follow, GameObject callingObject) {
		currentClipVolume = MusicVolume * GlobalVolume * sound.GetComponent<AudioSource>().volume;
		PlaySound(sound, follow, currentClipVolume, "Music", 1f, callingObject.transform);
	}

	private void PlaySound(GameObject sound, bool follow, float volume, string soundType, float pitch, Transform emitter) {
		SoundList.Add((GameObject) Instantiate(sound, emitter.position, Quaternion.identity));
		int i = SoundList.Count - 1;
		//GameObject go = ; //Instantiates the sound prefab at emitter position
		if (follow)
			SoundList[i].transform.parent = emitter; //Sets calling game object as parent so that the audio source follows it

		SoundManagementAddOn sMAO = SoundList[i].AddComponent<SoundManagementAddOn>();
		sMAO.NormalizedVolume = SoundList[i].GetComponent<AudioSource>().volume;
		sMAO.SoundType = soundType;
		SoundList[i].GetComponent<AudioSource>().volume = volume;
		SoundList[i].GetComponent<AudioSource>().pitch = pitch;
		SoundList[i].GetComponent<AudioSource>().Play();
		Destroy(SoundList[i], SoundList[i].GetComponent<AudioSource>().clip.length); //Destroys the instantiated object after the sound finishes playing
    }

	private void AdjustPlayingMusicVolume(int i) {
		SoundList[i].GetComponent<AudioSource>().volume = SoundList[i].GetComponent<SoundManagementAddOn>().NormalizedVolume * MusicVolume * GlobalVolume;
	}

	private void AdjustPlayingSFXVolume(int i) {
		SoundList[i].GetComponent<AudioSource>().volume = SoundList[i].GetComponent<SoundManagementAddOn>().NormalizedVolume * SFXVolume * GlobalVolume;
	}

    //	private bool Invariant(){ //if (Invariant()) {
    //		if ((1 >= globalVolume && globalVolume >= 0) && (1 >= musicVolume && musicVolume >= 0) && (1 >= sFXVolume && sFXVolume >= 0)) {
    //			return true;
    //		}
    //		else {
    //			Debug.Log (globalVolume);
    //			Debug.Log (musicVolume);
    //			Debug.Log (sFXVolume);
    //			return false;
    //		}
    //	}
}