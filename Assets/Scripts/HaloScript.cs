using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaloScript : MonoBehaviour {

	Light haloLight;
	float deltaIntensity;
	public float deltaValue;
	public float minIntensity;
	public float maxIntensity;
	public float selectedIntensity;
	private float currentSelectedIntensity;

	bool selected;
	void Awake(){
		haloLight = GetComponent<Light>();
		
		deltaIntensity = deltaValue;

		//if robot is selected, increase it's intensity
		currentSelectedIntensity = 0;
		selected = false;
		haloLight.enabled = false;
	}
	void Start(){
		 if(gameObject.name.Contains("Clone") == true)
			haloLight.enabled = false;
		else if(gameObject.GetComponent<RobotBehaviour>().isPreview == true){
			haloLight.enabled = false;
		}
	}
	void ToggleSelectedIntensity(GameObject r){
		
		if(r != gameObject){
			selected = false;
			return;
		}
			
		if(currentSelectedIntensity == 0){
	
			//currentSelectedIntensity = selectedIntensity;
			haloLight.intensity = selectedIntensity;
			selected = true;
		}
		else{haloLight.intensity = minIntensity;
			currentSelectedIntensity = 0;
			selected = false;

		}
	}
	// Update is called once per frame
	void Update () {
		if(selected == false){
			if(haloLight.intensity <= minIntensity){
			deltaIntensity = deltaValue;
		}
		else if(haloLight.intensity >= maxIntensity)
			deltaIntensity = -deltaValue;
		haloLight.intensity += deltaIntensity*Time.unscaledDeltaTime;
		}
		
	}
	 void OnEnable(){
		 if(gameObject.GetComponent<RobotBehaviour>().isPreview == false){
		
			RobotBehaviour.OnClick += ToggleSelectedIntensity;
	
			if(haloLight == null)
			{
				haloLight = GetComponent<Light>();
			}
			haloLight.enabled = true;
		 }
	}
	 void OnDisable(){
		 selected = false;
		haloLight.enabled = false;
		//ToggleSelectedIntensity(gameObject);
		RobotBehaviour.OnClick -= ToggleSelectedIntensity;
	}
	void OnDestroy(){
		RobotBehaviour.OnClick -= ToggleSelectedIntensity;
	}
}
