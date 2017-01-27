using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class MenuBehaviour : MonoBehaviour {
   
    public GameObject menuUIPrefab;
    public GameObject menuUIInstance;
 
    public delegate void PlayButtonClicked();
    public static event PlayButtonClicked OnPlayButtonClick;
    void Awake()
    {
        menuUIInstance = Instantiate(menuUIPrefab) as GameObject;
        
    }
    void Start () {
       
       
        
       
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    //sends event to gamebehaviour when it is time to start playing
    public void PressPlay()
    {


        OnPlayButtonClick();
    }
    public void PressQuit()
    {
        Application.Quit();
        //quitting?
    }
    public void Activate(bool b)
    {
        menuUIInstance.SetActive(b);
    }
}
