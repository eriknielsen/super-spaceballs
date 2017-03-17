using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateRandomObject : MonoBehaviour {

	[SerializeField]
	GameObject[] objectArray;

	void Start(){
		Instantiate(objectArray[Random.Range(0, objectArray.Length-1)], transform);
	}
}