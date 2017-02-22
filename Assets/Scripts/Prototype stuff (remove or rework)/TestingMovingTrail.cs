using UnityEngine;
using System.Collections;

public class TestingMovingTrail : MonoBehaviour
{
    bool hasBeenPressed = false;
    GameObject testObject;
    void Awake()
    {
        //testObject = new GameObject();
        //testObject.name = "testobject";
        //testObject.AddComponent<Animator>();
        //testObject.AddComponent<Rigidbody2D>();
        //testObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-7, 0);
        //Debug.Log(testObject.GetComponent<Rigidbody2D>().velocity);
        //Debug.Log(testObject.GetComponent<Rigidbody2D>());
        //testObject.AddComponent<RobotBehaviour>();
        //testObject.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
        
        //MoveCommand moveCommand = new MoveCommand(testObject, new Vector2(6, 3), 8.0f, 1);
        //testObject.GetComponent<RobotBehaviour>().Commands.Add(moveCommand);
        //testObject.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/fotball2");
        //testObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //MovingTrail trail = new MovingTrail(moveCommand);
        //testObject.GetComponent<RobotBehaviour>().CurrentState.EnterPlayState();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !hasBeenPressed)
        {
            testObject = new GameObject();
            testObject.name = "testobject";
            testObject.AddComponent<Animator>();
            testObject.AddComponent<Rigidbody2D>();
            testObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            Debug.Log(testObject.GetComponent<Rigidbody2D>().velocity);
            Debug.Log(testObject.GetComponent<Rigidbody2D>());
            testObject.AddComponent<RobotBehaviour>();
            testObject.GetComponent<Rigidbody2D>().gravityScale = 0.0f;

            Vector3 cursorPosition = Input.mousePosition;
            Vector3 cursorScreenPosition = Camera.main.ScreenToWorldPoint(cursorPosition);
            MoveCommand moveCommand = new MoveCommand(testObject, cursorScreenPosition, 8.0f, 1);
            testObject.GetComponent<RobotBehaviour>().Commands.Add(moveCommand);
            testObject.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/fotball2");
            testObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            MovingTrail trail = new MovingTrail(moveCommand, 0, testObject.GetComponent<Rigidbody2D>().velocity);
            testObject.GetComponent<RobotBehaviour>().CurrentState.EnterPlayState();
            Destroy(testObject);
            hasBeenPressed = true;
        }
        if (Input.GetMouseButtonDown(1))
        {
            Application.LoadLevel("Gilbert");
        }
    }
}
