using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayFieldMenu : MonoBehaviour {

	GameObject optionsMenu;

	void Start(){
		optionsMenu = GameObject.Find("IngameOptionsMenu");
	}

	public void MainMenu(){
		if (SceneManager.GetActiveScene().name == "OnlinePlay"){
			GameObject.Find("Matchmaker").GetComponent<ServerBehaviour>().LeaveToMainMenu();
		}
		SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
	}

	public void ToggleOptionsMenu(){
		optionsMenu.SetActive(!optionsMenu.activeInHierarchy);
	}

	public void Quit(){
		Application.Quit();
	}
}
