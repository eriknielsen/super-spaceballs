using UnityEngine;
using System.Collections;

public class TestingMovingTrail : MonoBehaviour
{

    GameObject testObject;
    void Awake()
    {
        testObject = new GameObject();
        testObject.AddComponent<Rigidbody2D>();
        testObject.GetComponent<Rigidbody2D>().velocity = new Vector2(2, 4);
        testObject.AddComponent<RobotBehaviour>();
        testObject.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
        MoveCommand moveCommand = new MoveCommand(testObject, new Vector2(-4, -3), 10.0f, 1);
        testObject.GetComponent<RobotBehaviour>().Commands.Add(moveCommand);
        testObject.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/fotball2");
        testObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        MovingTrail trail = new MovingTrail(moveCommand);
        testObject.GetComponent<RobotBehaviour>().CurrentState.EnterPlayState();
    }
}
