using UnityEngine;
using System.Collections;

public class MenuBehaviour : MonoBehaviour {

    GameObject menuPrefab;
	// instantiates the menu prefab as child
	void Start () {
        
        Debug.Log("menustart");
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("menu");
	}

    //sends event to gamebehaviour when it is time to start playing
}
