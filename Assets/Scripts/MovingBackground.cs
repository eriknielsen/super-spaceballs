using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A script for simple movement of background elements.
public class MovingBackground : MonoBehaviour {

	public int[] sortingOrder;
	public Vector3[] movementSpeed, rotationSpeed, scale;
	public Sprite[] sprites; //All arrays need to be matching lengths

	[SerializeField]
	float verticalBounds = 30f;
	[SerializeField]
	float horizontalBounds = 50f;
	[SerializeField]
	bool randomizeLocation;

	List<GameObject> movingObjects = new List<GameObject>();

	void Start(){
        gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
		if (sortingOrder.Length == (movementSpeed.Length + rotationSpeed.Length + rotationSpeed.Length + scale.Length) / 4){
			for (int i = 0; i < sprites.Length; i++){
				movingObjects.Add(new GameObject());
				movingObjects[i].transform.parent = transform;
				movingObjects[i].transform.position = new Vector3(Random.Range(-horizontalBounds, horizontalBounds), Random.Range(-verticalBounds, verticalBounds), 1);
				movingObjects[i].transform.localScale = scale[i];
				SpriteRenderer objectSR = movingObjects[i].AddComponent<SpriteRenderer>();
                movingObjects[i].GetComponent<SpriteRenderer>().sortingLayerName = "Background";
				objectSR.sprite = sprites[i];
				objectSR.sortingOrder = sortingOrder[i];
			}
		} else {
			Debug.Log ("Array lengths are not equal");
		}
	}

	void Update(){
		for (int i = 0; i < movingObjects.Count; i++){ //Move all objects by their respective speeds and wrap them back around if out of bounds
			movingObjects[i].transform.position = movingObjects[i].transform.position + movementSpeed[i] * Time.unscaledDeltaTime;
			movingObjects[i].transform.Rotate(rotationSpeed[i] * Time.unscaledDeltaTime);

			if (movingObjects[i].transform.position.x > horizontalBounds){
				HorizontalWrap(movingObjects[i], -horizontalBounds);
			} else if (movingObjects[i].transform.position.x < -horizontalBounds){
				HorizontalWrap(movingObjects[i], horizontalBounds);
			}
			if (movingObjects[i].transform.position.y > verticalBounds){
				VerticalWrap(movingObjects[i], -verticalBounds);
			} else if (movingObjects[i].transform.position.y < -verticalBounds){
				VerticalWrap(movingObjects[i], verticalBounds);
			}
		}
	}

	void HorizontalWrap(GameObject objectToMove, float x){
		float y;
		if (randomizeLocation) {
			y = Random.Range (-verticalBounds, verticalBounds);
		} else {
			y = objectToMove.transform.position.y;
		}
		objectToMove.transform.position = new Vector3(x, y, objectToMove.transform.position.z);
	}

	void VerticalWrap(GameObject objectToMove, float y){
		float x;
		if (randomizeLocation) {
			x = Random.Range (-horizontalBounds, horizontalBounds);
		} else {
			x = objectToMove.transform.position.x;
		}
		objectToMove.transform.position = new Vector3(x, y, objectToMove.transform.position.z);
	}
}
