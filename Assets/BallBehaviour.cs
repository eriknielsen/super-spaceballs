using UnityEngine;
using System.Collections;

public class BallBehaviour : MonoBehaviour {

    bool isPaused = false;
    LineRenderer pathDisplay;
    GameObject followMouse;

    void Awake()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(1, 0);
        pathDisplay = GetComponent<LineRenderer>();
        pathDisplay.SetPosition(1, new Vector3(10, 0, 0));
        followMouse = new GameObject();
    }

	void OnMouseDown()
    {
        if (!isPaused)
        {
            Time.timeScale = 0;
            isPaused = true;
        }
        else
        {
            Time.timeScale = 1;
            isPaused = false;
        }
    }

    bool isFollowing = false;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isFollowing)
            {
                isFollowing = true;
            }
            else
            {
                isFollowing = false;
            }
        }
        if (isFollowing)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f; // Set this to be the distance you want the object to be placed in front of the camera.
            pathDisplay.SetPosition(1, Camera.main.ScreenToWorldPoint(mousePos));
        }

        //Vector3 temp = Input.mousePosition;
        //temp.z = 10f; // Set this to be the distance you want the object to be placed in front of the camera.
        //this.transform.position = Camera.main.ScreenToWorldPoint(temp);
    }
}
