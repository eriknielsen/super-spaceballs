using UnityEngine;
using System.Collections;

//This is a singleton that will act as an interface between the implementation of sound-handling and the scripts requesting audio to be played.
//Example of how to call the API:
// Add a reference to the game object prefab that contains the audio you want to play: public GameObject SoundObjectVariableNameHere;
// Which obviously means you need an audio prefab. You can create one by simply dragging an audio file into the Hierarchy and then dragging the resulting object into the prefabs/sounds folder.
// Call the function you want to use through the API: SoundManagerAPI.instance.PlaySFX(SoundObjectVariableNameHere, true, this.gameObject);
// Where "this.gameObject" should never be changed.

//NOT YET A COMPLETED SERVICE LOCATOR IMPLEMENTATION

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
		ResetVolume();
	}

	public void ResetVolume(){
		soundManager.ResetVolume();
	}

	public void PlaySFX(GameObject sound, bool follow, GameObject callingObject){
		soundManager.PlaySFX(sound, follow, callingObject);
	}

	public void PlayMusic(GameObject sound, bool follow, GameObject callingObject) {
		soundManager.PlayMusic(sound, follow, callingObject);
    }

	public void PlayLoopingSFX(GameObject sound, bool follow, GameObject callingObject) {
		soundManager.PlaySFX(sound, follow, callingObject);
    }

	public void PlayLoopingMusic(GameObject sound, bool follow, GameObject callingObject) {
		soundManager.PlaySFX(sound, follow, callingObject);
    }
}
