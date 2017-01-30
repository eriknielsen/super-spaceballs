using UnityEngine;
using System.Collections;

public class VolumeManager : MonoBehaviour{

    [Range(0, 1)]
    public float defaultGlobalVolume;
    [Range(0, 1)]
    public float defaultMusicVolume;
    [Range(0, 1)]
    public float defaultSFXVolume;

    private float globalVolume;
    private float musicVolume;
    private float sFXVolume;

    public void ResetVolume() {
        globalVolume = defaultGlobalVolume;
        musicVolume = defaultMusicVolume;
        sFXVolume = defaultSFXVolume;
    }

    //if (volumeManager != null)
    //    volumeManager.ResetVolume();
    //else Debug.Log("Error: No volume manager added");
}