using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public GameObject optionsMenu;
	public GameObject mainButtons;
	public GameObject loadingScreen;

	MainMenuMusic mainMenuMusic;

	void Start(){
		mainMenuMusic = FindObjectOfType<MainMenuMusic>();
	}

	public void LocalPlay(){
		SceneManager.LoadSceneAsync("LocalPlay", LoadSceneMode.Single);
		ShowLoadingScreen();
	}

	public void OnlinePlay(){
		SceneManager.LoadSceneAsync("OnlinePlay", LoadSceneMode.Single);
		ShowLoadingScreen();
	}

	public void ToggleOptionsMenu(){
		mainButtons.SetActive(!mainButtons.activeInHierarchy);
		optionsMenu.SetActive(!optionsMenu.activeInHierarchy);
	}

	public void Quit(){
		Application.Quit();
	}

	void ShowLoadingScreen(){
		loadingScreen.SetActive(true);
		StartCoroutine(AudioManager.Instance.FadeSound(mainMenuMusic.gameObject.GetComponent<AudioSource>(), 5));
	}
}