using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayFieldMenu : MonoBehaviour {

	[SerializeField]
	GameObject optionsMenu;

	void Start(){
		//optionsMenu = GameObject.Find("IngameOptionsMenu");
	}

	public void MainMenu(){
		if (SceneManager.GetActiveScene().name == "OnlinePlay"){
			GameObject.Find("Matchmaker").GetComponent<ServerBehaviour>().LeaveToMainMenu();
		}
		SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
	}

	public void ToggleOptionsMenu(){
		Debug.Log(optionsMenu);
		optionsMenu.SetActive(!optionsMenu.activeInHierarchy);
	}

	public void Quit(){
		Debug.Log("quitting");
		Application.Quit();
	}
}
