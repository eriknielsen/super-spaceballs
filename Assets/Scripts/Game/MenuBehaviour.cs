using UnityEngine;
using System.Collections;

public class MenuBehaviour : MonoBehaviour {

	// instantiates the menu prefab
	void Start () {
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("menu");
	}

    //sends event to gamebehaviour when it is time to start playing
}
