using UnityEngine;
using System.Collections;

//This is a singleton that will act as an interface between the implementation of sound-handling and the scripts requesting audio to be played.
//Example of how to call the API:
//  AudioSource audio = GetComponent<AudioSource>();
//  SoundManagerAPI.instance.PlaySFX(audio);

public class SoundManagerAPI : MonoBehaviour{

	public static SoundManagerAPI instance = null;

	void Awake(){
		if (instance == null) 
			instance = this;
		else if (instance != this) { //Makes sure there's only one instance of the script
			Destroy (gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}

    private ArsonisticSoundManager soundManager;

    void Start(){
        if (soundManager == null)
			soundManager = GetComponent<ArsonisticSoundManager>();
        soundManager.ResetVolume();
	}

	public void PlaySFX(AudioSource sound){
        soundManager.PlaySFX(sound);
	}

    public void PlayMusic(AudioSource sound) {
        soundManager.PlayMusic(sound);
    }

}
