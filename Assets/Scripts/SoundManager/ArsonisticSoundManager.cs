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

    public void ResetVolume() {
        globalVolume = defaultGlobalVolume;
        musicVolume = defaultMusicVolume;
        sFXVolume = defaultSFXVolume;
    }

	public void PlaySFX(GameObject sound, bool follow, GameObject callingObject) {
		currentClipVolume = sFXVolume * globalVolume * sound.GetComponent<AudioSource>().volume;
		PlaySound(sound, follow, currentClipVolume, 1f, callingObject.transform);
    }

	public void PlayReptitiveSFX(GameObject sound, bool follow, GameObject callingObject) {
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
		currentClipVolume = sFXVolume * globalVolume * sound.GetComponent<AudioSource>().volume;
		PlaySound(sound, follow, currentClipVolume, randomPitch, callingObject.transform);
    }

	public void PlayMusic(GameObject sound, bool follow, GameObject callingObject) {
		currentClipVolume = musicVolume * globalVolume * sound.GetComponent<AudioSource>().volume;
		PlaySound(sound, follow, currentClipVolume, 1f, callingObject.transform);
	}


	private void PlaySound(GameObject sound, bool follow, float volume, float pitch, Transform emitter) {
		GameObject go = (GameObject) Instantiate(sound, emitter.position, Quaternion.identity); //Instantiates the sound prefab at emitter position
		if (follow)
			go.transform.parent = emitter; //Sets calling game object as parent so that the audio source follows it
		
		go.GetComponent<AudioSource>().volume = volume;
		go.GetComponent<AudioSource>().pitch = pitch;
		go.GetComponent<AudioSource>().Play();
		Destroy(go, go.GetComponent<AudioSource>().clip.length); //Destroys the new game object after the sound finishes playing
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