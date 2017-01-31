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
// Example: ArsonisticSoundManager.instance.GlobalVolume = 

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
	public float GlobalVolume { get; set; }
	public float MusicVolume { get; set; }
	public float SFXVolume { get; set; }

    private float currentClipVolume;
	//private List <GameObject> musicList;
	private System.Collections.Generic.List<GameObject> musicList = new System.Collections.Generic.List<GameObject>();

    public void ResetVolume() {
		GlobalVolume = defaultGlobalVolume;
		MusicVolume = defaultMusicVolume;
		SFXVolume = defaultSFXVolume;
    }

	public void GlobalVolumeTransition() {
		//GlobalVolume = ;
	}

	public void MusicVolumeTransition() {
		//MusicVolume = ;
	}

	public void SFXVolumeTransition() {
		//SFXVolume = ;
	}

	public void PlaySFX(GameObject sound, bool follow, GameObject callingObject) {
		currentClipVolume = SFXVolume * GlobalVolume * sound.GetComponent<AudioSource>().volume;
		PlaySound(sound, follow, currentClipVolume, 1f, callingObject.transform);
    }

	public void PlayReptitiveSFX(GameObject sound, bool follow, GameObject callingObject) {
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
		currentClipVolume = SFXVolume * GlobalVolume * sound.GetComponent<AudioSource>().volume;
		PlaySound(sound, follow, currentClipVolume, randomPitch, callingObject.transform);
    }

	public void PlayMusic(GameObject sound, bool follow, GameObject callingObject) {
		currentClipVolume = MusicVolume * GlobalVolume * sound.GetComponent<AudioSource>().volume;
		PlaySound(sound, follow, currentClipVolume, 1f, callingObject.transform);
	}


	private void PlaySound(GameObject sound, bool follow, float volume, float pitch, Transform emitter) {
		musicList.Add((GameObject) Instantiate(sound, emitter.position, Quaternion.identity));
		int i = musicList.Count - 1;
		//GameObject go = ; //Instantiates the sound prefab at emitter position
		if (follow)
			musicList[i].transform.parent = emitter; //Sets calling game object as parent so that the audio source follows it
		
		musicList[i].GetComponent<AudioSource>().volume = volume;
		musicList[i].GetComponent<AudioSource>().pitch = pitch;
		musicList[i].GetComponent<AudioSource>().Play();
		Destroy(musicList[i], musicList[i].GetComponent<AudioSource>().clip.length); //Destroys the instantiated object after the sound finishes playing
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