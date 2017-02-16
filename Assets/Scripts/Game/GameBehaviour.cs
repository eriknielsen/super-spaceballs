using UnityEngine;
using System.Collections;

public class GameBehaviour : MonoBehaviour {

    //gamebehaviour decides which behaviour should be ran
    //either the menubehaviour or the playbehaviour
    
    State currentState;
    GameObject LocalPlayInstance;
    GameObject MainMenuInstance;
    public enum State { Menu, LocalPlay, NetworkPlay };

    
    public GameObject MenuPrefab;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    
        MainMenuInstance = Instantiate(MenuPrefab);
        MainMenuInstance.transform.parent = gameObject.transform;
        EnterMenu();
        MenuBehaviour.OnPlayButtonClick += new MenuBehaviour.PlayButtonClicked(EnterLocalPlay);
        PlayBehaviour.OnReturnMenuButtonClick += new PlayBehaviour.ReturnMenuButtonClicked(EnterMenu);
        PlayBehaviour.OnReplayButtonClick += new PlayBehaviour.ReplayButtonClicked(LocalPlayReplayLastTurn);

    }

    public void EnterLocalPlay()
    {
        //do stuff to make local play happen

        MainMenuInstance.GetComponent<MenuBehaviour>().Activate(false);
        //LocalPlayInstance.GetComponent<PlayBehaviour>().enabled = true;

        LocalPlayInstance.GetComponent<PlayBehaviour>().Activate(true);        
        
    }

    public void EnterMenu()
    {

        //let localplay handle itself
        LocalPlayInstance.GetComponent<PlayBehaviour>().Activate(false);
        //let mainmenu activate itself
        MainMenuInstance.GetComponent<MenuBehaviour>().Activate(true);
    }
    void LocalPlayReplayLastTurn()
    {
        LocalPlayInstance.GetComponent<PlayBehaviour>().ReplayLastTurn();
    }
  
}
