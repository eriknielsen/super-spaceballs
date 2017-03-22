using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuOnlinePlayButton : MonoBehaviour {

	private MainMenu menuHandler;

    void Start()
    {
        GetComponent<Button>().colors = ToolBox.Instance.ButtonColors;
        menuHandler = FindObjectOfType<MainMenu>();
    }

    public void OnClick(){
		menuHandler.OnlinePlay();
	}
}