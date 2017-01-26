using UnityEngine;
using System.Collections;

public class GameBehaviour : MonoBehaviour {

    //gamebehaviour decides which behaviour should be ran
    //either the menubehaviour or the playbehaviour
    
    State currentState;
    GameObject LocalPlayInstance;
    GameObject MainMenuInstance;
    public enum State { Menu, LocalPlay, NetworkPlay };

    public GameObject LocalPlayPrefab;
    public GameObject MenuPrefab;

    void Awake()
    {
        LocalPlayInstance = Instantiate(LocalPlayPrefab);
        LocalPlayInstance.transform.parent = gameObject.transform;
        MainMenuInstance = Instantiate(MenuPrefab);
        MainMenuInstance.transform.parent = gameObject.transform;
        MenuBehaviour.OnPlayButtonClick += new MenuBehaviour.PlayButtonClicked(EnterLocalPlay);
        PlayBehaviour.OnReturnMenuButtonClick += new PlayBehaviour.ReturnMenuButtonClicked(EnterMenuFromPlay);
    }
    // Use this for initialization
    void Start () {

        //instansera soundmanager
        //currentState = State.Menu;

        //listen for clicking on the playbutton in menu
        //MainMenuInstance.GetComponent<MenuBehaviour>().PressPlay();
        
        //listen for return to menu button in play
        //PlayBehaviour.OnReturnMenuButtonClick += new PlayBehaviour.ReturnMenuButtonClicked(EnterMenuFromPlay);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void EnterLocalPlay()
    {

        //do stuff to make local play happen
     

        Debug.Log(LocalPlayInstance);
        Debug.Log(MainMenuInstance);
        MainMenuInstance.GetComponent<MenuBehaviour>().Activate(false);
        //LocalPlayInstance.GetComponent<PlayBehaviour>().enabled = true;
        LocalPlayInstance.GetComponent<PlayBehaviour>().Activate(true);
        
        
        
    }
    public void EnterMenuFromPlay()
    {
        Debug.Log(LocalPlayInstance);
        //let localplay handle itself
        LocalPlayInstance.GetComponent<PlayBehaviour>().Activate(false);
        //let mainmenu activate itself
        MainMenuInstance.GetComponent<MenuBehaviour>().Activate(true);
    }
}
