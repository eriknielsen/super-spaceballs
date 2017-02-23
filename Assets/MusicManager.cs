using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {


    public GameObject song1;
    public GameObject song2;
    public GameObject song3;
    // Use this for initialization
    void Start () {
        AudioManager.instance.PlayPersistentMusic(song1);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
