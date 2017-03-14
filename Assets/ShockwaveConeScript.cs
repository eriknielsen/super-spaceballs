﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveConeScript : MonoBehaviour {

	Rigidbody2D rb;
	Vector2 lastNodePosition;
	Vector2 point;
	Vector2 pointToCompareWith;

	SpriteRenderer sp;

	void Awake () {
		rb = GetComponent<Rigidbody2D>();
		lastNodePosition = Vector2.zero;
		sp = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
/* 
	public void f(Vector2 point, GameObject pointToCompareWith, Vector2 hej){


		//get direction from point and gameObject.transform.position
		if(hej != Vector2.zero)
			transform.position = hej;
		else
			transform.position = pointToCompareWith.transform.position;
		enabled = true;
		Vector3 diff = (Vector3)point - pointToCompareWith.transform.position; 

        float rot_z = (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
        rb.rotation = rot_z;
        
       
	}
	*/
	void g(){
		sp.enabled = true;
		if(lastNodePosition == Vector2.zero){
			lastNodePosition = pointToCompareWith;
		}
		transform.position = lastNodePosition;
		Vector3 diff = point - pointToCompareWith; 

        float rot_z = (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
        rb.rotation = rot_z;
	}
	public void SetPositions(Vector2 cursorTextPoint, Vector2 robotPos){
		pointToCompareWith = robotPos;
		point = cursorTextPoint;
	}
	public void SetConePosition(Vector2 pos){
		lastNodePosition = pos;
		g();
	}
	public void DeActivateSprite(){
		sp.enabled = false;
	}
}
