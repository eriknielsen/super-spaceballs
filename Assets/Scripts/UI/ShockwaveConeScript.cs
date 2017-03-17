using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveConeScript : MonoBehaviour {

	Rigidbody2D rb;
	Vector2 lastPreviewPosition;
	Vector2 point;
	Vector2 pointToCompareWith;

	SpriteRenderer sr;

	void Awake(){
		rb = GetComponent<Rigidbody2D>();
		lastPreviewPosition = Vector2.zero;
		sr = GetComponent<SpriteRenderer>();
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
		sr.enabled = true;
		if(lastPreviewPosition == Vector2.zero){
			lastPreviewPosition = pointToCompareWith;
		}
		transform.position = lastPreviewPosition;
		Vector3 diff = point - pointToCompareWith; 

        float rot_z = (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
        rb.rotation = rot_z;
	}

	public void SetPositions(Vector2 cursorTextPoint, Vector2 robotPos){
		pointToCompareWith = robotPos;
		point = cursorTextPoint;
	}

	public void SetConePosition(Vector2 pos){
		lastPreviewPosition = pos;
		g();
	}

	public void DeActivateSprite(){
		sr.enabled = false;
	}
}
