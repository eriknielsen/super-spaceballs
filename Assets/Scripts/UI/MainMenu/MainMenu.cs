using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public GameObject optionsMenu;
	public GameObject mainButtons;


	public void LocalPlay(){
		SceneManager.LoadSceneAsync("LocalPlay", LoadSceneMode.Single);
	}

	public void OnlinePlay(){
		SceneManager.LoadSceneAsync("OnlinePlay", LoadSceneMode.Single);
	}

	public void ToggleOptionsMenu(){
		mainButtons.SetActive(!mainButtons.activeInHierarchy);
		optionsMenu.SetActive(!optionsMenu.activeInHierarchy);
	}

	public void Quit(){
		Application.Quit();
	}
}
