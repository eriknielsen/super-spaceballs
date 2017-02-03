using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public GameObject optionsMenu;
	public GameObject mainButtons;

	public void LocalPlay(){
		SceneManager.LoadSceneAsync("GameField", LoadSceneMode.Single);
	}

	public void OnlinePlay(){
		//Debug.Log(SceneManager.GetActiveScene().buildIndex)
		SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
	}

	public void ToggleOptionsMenu(){
		mainButtons.SetActive(!mainButtons.activeInHierarchy);
		optionsMenu.SetActive(!optionsMenu.activeInHierarchy);
	}

	public void Quit(){
		Application.Quit();
	}
}
