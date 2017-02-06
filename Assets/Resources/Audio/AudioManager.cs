using UnityEngine;
using System.Collections;
//using UnityEngine.UI;
//using UnityEngine.Audio;

// This is a singleton that will spawn new sounds through function calls. Ambient sounds that always are present should simply be attached to their respective gameobjects.
// Instructions:
// Add a reference to the game object prefab that contains the audio you want to play: public GameObject SoundObjectVariableNameHere;
// Which obviously means you need an audio prefab. You can create one by dragging an audio file into the Hierarchy, dragging the resulting object into the audio/prefabs folder and deleting the object in the hierarchy
// Call the function you want to use: ArsonisticSoundManager.instance.PlayAudio(SoundObjectVariableNameHere, false, this.gameObject);
// Where "SoundObjectVariableNameHere" is a variable name of your choice (just needs to match variable declaration), "false" is whether the sound will follow the object emitting the sound
// Set to true in case of for example music playing from a car-radio in the world. Also set it to true if using 2D sounds, to keep the hierarchy clear.
// and "this.gameObject" SHOULD NOT BE TOUCHED. It's a reference to the game object containing the script; the sound manager needs to know what object is creating the sound to be able to localize it.

public class AudioManager : MonoBehaviour {

	public static AudioManager instance = null;
    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this) { //Makes sure there's only one instance of the script
            Destroy(gameObject); //Goes nuclear
        }
        DontDestroyOnLoad(gameObject);
    }

    //public float minimumTimeBetweenSFX; //Necessary?
    public float highPitchRange = 1.05f;
    public float lowPitchRange = 0.95f;

	public void PlayAudio(GameObject soundObject, bool follow, GameObject callingObject) {
		PlaySound(soundObject, follow, 1f, callingObject.transform);
    }

	public void PlayAudioWithRandomPitch(GameObject soundObject, bool follow, GameObject callingObject) {
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
		PlaySound(soundObject, follow, randomPitch, callingObject.transform);
    }


	// --------------
	// PRIVATE
	// --------------

	private void PlaySound(GameObject soundObject, bool follow, float pitch, Transform emitter) {
		GameObject go = (GameObject) Instantiate(soundObject, emitter.position, Quaternion.identity); //Instantiates the sound prefab at emitter position
		if (follow)
			go.transform.parent = emitter; //Sets calling game object as parent so that the audio source follows it

		go.GetComponent<AudioSource>().pitch = pitch;
		go.GetComponent<AudioSource>().Play();
		Destroy(go, go.GetComponent<AudioSource>().clip.length); //Destroys the instantiated object after the sound finishes playing
    }
}