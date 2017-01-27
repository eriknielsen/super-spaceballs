using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pattern : MonoBehaviour {

    Sprite nodeSprite, dotSprite;

    List<GameObject> nodes, dots;
    Vector2 startPosition, endPosition;
    GameObject movingObject;
    Vector2 movingObjectForce;

    Pattern(GameObject movingObject, Vector2 startPos, Vector2 endPos, Vector2 force)
    {
        this.movingObject = movingObject;
        startPosition = startPos;
        endPosition = endPos;
        movingObjectForce = force;
    }

    void Awake()
    {
        nodes = new List<GameObject>();
        dots = new List<GameObject>();

        nodes.Add(new GameObject());
        nodes[0].transform.position = startPosition;
        nodes[0].AddComponent<LineRenderer>();
    }

	public void ExtendPattern(Pattern pattern)
    {
        StartCoroutine(PlacingNode());
    }

    IEnumerator PlacingNode()
    {
        bool isPlaced = false;
        while (!isPlaced) {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f; // Set this to be the distance you want the object to be placed in front of the camera.
            if (nodes.Count > 0)
            {
                
            }
        }
        yield return null;
    }
}
