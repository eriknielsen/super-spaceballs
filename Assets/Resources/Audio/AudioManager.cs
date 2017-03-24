using UnityEngine;
using System.Collections;
//using UnityEngine.UI;
//using UnityEngine.Audio;

// This is a singleton that will spawn new sounds through function calls. Ambient sounds that always are present should simply be attached to their respective gameobjects.
// Instructions:
// Add a reference to the game object prefab that contains the audio you want to play: public GameObject SoundObjectVariableNameHere;
// Which obviously means you need an audio prefab. You can create one by dragging an audio file into the Hierarchy, dragging the resulting object into the audio/prefabs folder and deleting the object in the hierarchy
// Call the function you want to use: AudioManager.Instance.PlayAudio(SoundObjectVariableNameHere, false, gameObject);
// Where "SoundObjectVariableNameHere" is a variable name of your choice (just needs to match variable declaration), "false" is whether the sound will follow the object emitting the sound
// Set to true in case of for example music playing from a car-radio in the world. Also set it to true if using 2D sounds, to keep the hierarchy clear.
// and "gameObject" SHOULD NOT BE TOUCHED. It's a reference to the game object containing the script; the sound manager needs to know what object is creating the sound to be able to localize it.
// You can also call on the fade sound function through: StartCoroutine(AudioManager.Instance.FadeSound(audioSourceVariableNameHere, timeInSecondsToFade));
// If you want the ability to terminate the fade early you can do that by declaring a Coroutine variable and assigning it the started coroutine
// like this: fadeSound = StartCoroutine(AudioManager.Instance.FadeSound(audioSourceVariableNameHere, timeInSecondsToFade)); then you can Stop it with: StopCoroutine(fadeSound);

public class AudioManager : MonoBehaviour {

	public static AudioManager Instance = null;

	public float randomPitchDeviation = 0.05f;
	public bool unscaledTime = false;


	float fadeTimeRemaining;
	Coroutine fadeMusic;


    void Awake(){
        if (Instance == null)
            Instance = this;
        else if (Instance != this){ //Makes sure there's only one instance of the script
            Destroy(gameObject); //Goes nuclear
			return;
        }
        DontDestroyOnLoad(gameObject);
    }

	public IEnumerator FadeSound(AudioSource audioSource, float fadeTime){
		float startVolume = audioSource.volume;
		audioSource.volume = -1; //TEST
		fadeTimeRemaining = fadeTime;
		while (fadeTimeRemaining >= 0){
			if (unscaledTime)
				fadeTimeRemaining -= Time.unscaledDeltaTime;
			else
				fadeTimeRemaining -= Time.deltaTime;
			audioSource.volume = Mathf.Clamp01(fadeTimeRemaining/fadeTime) * startVolume;
			yield return null;
		}
	}

	public void PlayAudio(GameObject soundObject, bool follow, GameObject callingObject){
		PlaySound(soundObject, follow, 1f, callingObject.transform);
    }

    public void PlayAudioWithRandomPitch(GameObject soundObject, bool follow, GameObject callingObject){
		float randomPitch = Random.Range(1-randomPitchDeviation, 1+randomPitchDeviation);
        
        PlaySound(soundObject, follow, randomPitch, callingObject.transform);
    }

	void PlaySound (GameObject soundObject, bool follow, float pitch, Transform emitter){ //Maybe return a reference to the object for higher control after instantiation?
		GameObject go = (GameObject) Instantiate(soundObject, emitter.position, Quaternion.identity); //Instantiates the sound prefab at emitter position
		if (follow)
			go.transform.parent = emitter; //Sets calling game object as parent so that the audio source follows it
        
		go.GetComponent<AudioSource>().pitch = pitch;
		go.GetComponent<AudioSource>().Play();
        
		if (unscaledTime)
			StartCoroutine(DestroyAfterUnscaledTime(go, go.GetComponent<AudioSource>().clip.length)); //Destroys the instantiated object after the sound finishes playing
		else
			Destroy(go, go.GetComponent<AudioSource>().clip.length);
    }

	IEnumerator DestroyAfterUnscaledTime(GameObject go, float time){ //Unless you change sound pitch the end of the sound will not match the destruction of the object when using the built in delay in the Destroy function with a time scale that isn't 1
		yield return new WaitForSecondsRealtime(time);
		if (go != null)
			Destroy(go);
	}

	void OnDestroy(){
		StopAllCoroutines();
	}
}