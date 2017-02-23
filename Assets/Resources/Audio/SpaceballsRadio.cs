using UnityEngine;
using System.Collections;

public class SpaceballsRadio : MonoBehaviour { //Basic music player for now

	[HideInInspector]
	public AudioSource audioSource;

	void Start(){
		audioSource = GetComponent<AudioSource>();
		audioSource.Play();
	}
}
