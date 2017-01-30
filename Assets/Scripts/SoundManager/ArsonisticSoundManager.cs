using UnityEngine;
using System.Collections;

//This handles volume management and the actual playing of the sounds.

public class ArsonisticSoundManager : MonoBehaviour {

    [Range(0, 1)]
    public float defaultGlobalVolume;
    [Range(0, 1)]
    public float defaultMusicVolume;
    [Range(0, 1)]
    public float defaultSFXVolume;
    public float minimumTimeBetweenSFX;
    public float highPitchRange = 1.05f;
    public float lowPitchRange = 0.95f;

    private float globalVolume;
    private float musicVolume;
    private float sFXVolume;
    private float currentClipVolume;

    void Start() {
        ResetVolume();
	}

    public void ResetVolume() {
        globalVolume = defaultGlobalVolume;
        musicVolume = defaultMusicVolume;
        sFXVolume = defaultSFXVolume;
    }

    public void PlaySFX(AudioSource sound, GameObject callingObject, bool follow) {
        currentClipVolume = sFXVolume * globalVolume * sound.volume;
        PlaySound(sound, callingObject.transform, follow, currentClipVolume, 1f);
    }

	public void PlayMusic(AudioSource sound, GameObject callingObject, bool follow) {
		currentClipVolume = musicVolume * globalVolume * sound.volume;
		PlaySound(sound, callingObject.transform, follow, currentClipVolume, 1f);
	}

	public void PlayReptitiveSFX(AudioSource sound, GameObject callingObject, bool follow) {
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        currentClipVolume = sFXVolume * globalVolume * sound.volume;
        //PlaySound(sound, callingObject.transform, follow, currentClipVolume);
    }

    public void PlaySound(AudioSource sound, Transform sourceTransform, bool follow, float volume, float pitch) {
        GameObject go = new GameObject("Audio: " + sound.clip.name);    //Creates a new empty game object
        go.transform.position = sourceTransform.position;               //Moves the new game object to location of the calling game object
        if (follow)
            go.transform.parent = sourceTransform;                      //Sets calling game object as parent so that the audio source follows it

        AudioSource source = go.AddComponent<AudioSource>();            //Creates and adds an audio source to the new game object

        //source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
        //Destroy(go, clip.length);                                       //Destroys the new game object after the sound finishes playing
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