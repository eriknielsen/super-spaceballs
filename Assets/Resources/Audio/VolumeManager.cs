using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour {

	public AudioMixer masterMixer;
	public Text masterVolumeText;
	public Text musicVolumeText;
	public Text sFXVolumeText;

	public float decibelRange = 40f;

	void Awake(){
		if (PlayerPrefs.GetInt("StartedBefore") == 0) { //Checks if this is the first time the game has been started
			SetMasterVolume(100f);
			SetMusicVolume(100f);
			SetSFXVolume(100f);
			PlayerPrefs.SetInt("StartedBefore", 1);
		}
	}

	public void SetMasterVolume(float masterVol){
		if (masterVol == 0){
			masterMixer.SetFloat("masterVolume", -80f);
		} else
			masterMixer.SetFloat("masterVolume", VolumeRangeAdjustment(masterVol));
		masterVolumeText.text = ("" + masterVol);
		PlayerPrefs.SetFloat("MasterVolume", masterVol);
	}

	public void SetMusicVolume(float musicVol){
		if (musicVol == 0){
			masterMixer.SetFloat("musicVolume", -80f);
		} else
			masterMixer.SetFloat("musicVolume", VolumeRangeAdjustment(musicVol));
		musicVolumeText.text = ("" + musicVol);
		PlayerPrefs.SetFloat("MusicVolume", musicVol);
	}

	public void SetSFXVolume(float sFXVol){
		if (sFXVol == 0){
			masterMixer.SetFloat("sFXVolume", -80f);
		} else
			masterMixer.SetFloat("sFXVolume", VolumeRangeAdjustment(sFXVol));
		sFXVolumeText.text = ("" + sFXVol);
		PlayerPrefs.SetFloat("SFXVolume", sFXVol);
	}

	public float VolumeRangeAdjustment(float inputVol){
		float finalVolume = inputVol * (decibelRange * 0.01f) - decibelRange;
		return finalVolume;
	}
}