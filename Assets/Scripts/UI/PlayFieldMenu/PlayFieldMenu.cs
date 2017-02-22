using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayFieldMenu : MonoBehaviour {

	public GameObject optionsMenu;

	public void MainMenu(){
		SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
	}

	public void ToggleOptionsMenu(){
		optionsMenu.SetActive(!optionsMenu.activeInHierarchy);
	}

	public void Quit(){
		Application.Quit();
	}
}
