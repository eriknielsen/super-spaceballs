using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

    public float highPitchRange = 1.05f;
    public float lowPitchRange = 0.95f;

    void Start() {
        
	}

    public void PlaySFX(AudioSource sound) {
        PlaySound.playSound(sound.volume * sFXVolume * globalVolume);
		sound.Play();
	}

    public void PlayReptitiveSFX(AudioSource sound) {
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        Play.pitch = randomPitch;
        PlaySound.playSound(sound.volume * sFXVolume * globalVolume);
        sound.Play();
    }

    public void PlayMusic(AudioSource sound) {
		sound.volume = sound.volume * musicVolume * globalVolume;
		sound.Play();
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
