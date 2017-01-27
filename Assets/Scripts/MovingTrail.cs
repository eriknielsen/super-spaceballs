using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MovingTrail : MonoBehaviour
{

    Sprite nodeSprite, trailMarkingSprite;

    List<SubTrail> trails;
    Vector2 startPosition, endPosition;
    GameObject movingObject;
    Vector2 movingObjectForce;

    class SubTrail
    {
        GameObject node;
        List<GameObject> trailMarkings;
        MoveCommand moveCommand;

        Sprite nodeSprite, trailMarkingSprite;

        public SubTrail(MoveCommand moveCommand, Sprite nodeSprite, Sprite trailMarkingSprite)
        {
            node = new GameObject();
            node.name = "Node";
            trailMarkings = new List<GameObject>();
            this.nodeSprite = nodeSprite;
            this.trailMarkingSprite = trailMarkingSprite;
            this.moveCommand = moveCommand;
            if (nodeSprite == null || trailMarkingSprite == null)
            {
                Debug.LogWarning("Sprite for node or trailMarking is null.");
            }
            MakeTrail();
        }

        void MakeTrail()
        {
            Vector2 lastPosition = moveCommand.Robot.transform.position;
            Vector2 lastSpeed = moveCommand.Robot.GetComponent<Rigidbody2D>().velocity;
            float duration = moveCommand.LifeDuration;
            float timeInterval = 0.1f;
            Vector2 acceleration = moveCommand.ResultingForce / moveCommand.robot.GetComponent<Rigidbody2D>().mass;
            for (float timeStamp = timeInterval; timeStamp < duration; timeStamp += timeInterval)
            {
                Vector2 resultingPosition = GetResultingPosition(lastPosition, lastSpeed, acceleration, timeStamp);
                lastPosition = resultingPosition;
                lastSpeed = GetResultingSpeed(lastSpeed, acceleration, timeStamp);
                if (timeStamp >= duration)
                {
                    node.AddComponent<SpriteRenderer>();
                    node.GetComponent<SpriteRenderer>().sprite = nodeSprite;
                    node.transform.position = resultingPosition;
                    if (nodeSprite != null)
                    {
                        float scaleFactor = 1 / nodeSprite.bounds.size.x;
                        node.transform.localScale = new Vector3(scaleFactor, scaleFactor);
                    }
                    break;
                }

                trailMarkings.Add(new GameObject());
                trailMarkings.Last().transform.position = resultingPosition;
                trailMarkings.Last().AddComponent<SpriteRenderer>();
                trailMarkings.Last().GetComponent<SpriteRenderer>().sprite = trailMarkingSprite;
                trailMarkings.Last().transform.parent = node.transform;
                trailMarkings.Last().name = "Trail Marking";
                if (trailMarkingSprite != null)
                {
                    float scaleFactor = 1 / trailMarkingSprite.bounds.size.x;
                    trailMarkings.Last().transform.localScale = new Vector3(scaleFactor, scaleFactor);
                }
            }
        }

        Vector2 GetResultingPosition(Vector2 position, Vector2 speed, Vector2 acceleration, float time)
        {
            float xPosition = position.x + speed.x * time + (acceleration.x + Mathf.Pow(time, 2)) / 2;
            float yPosition = position.y + speed.y * time + (acceleration.y + Mathf.Pow(time, 2)) / 2;
            Vector2 resultingPosition = new Vector2(xPosition, yPosition);
            return resultingPosition;
        }

        Vector2 GetResultingSpeed(Vector2 speed, Vector2 acceleration, float time)
        {
            Vector2 resultingSpeed = speed + acceleration * time;
            return resultingSpeed;
        }
    }

    MovingTrail(GameObject movingObject, Vector2 startPos, Vector2 endPos)
    {
        this.movingObject = movingObject;
        startPosition = startPos;
        endPosition = endPos;
    }

    void Awake()
    {
        trails = new List<SubTrail>();
        nodeSprite = Resources.Load<Sprite>("Sprites/fotball2");
        trailMarkingSprite = Resources.Load<Sprite>("Sprites/fotball2");
        GameObject g = new GameObject();
        g.AddComponent<Rigidbody2D>();
        g.AddComponent<Rigidbody2D>();
        g.GetComponent<Rigidbody2D>().velocity = new Vector2(-10, 0);
        MoveCommand command = new MoveCommand(g, g.transform.position + new Vector3(6, 2), 2, 1);
        trails.Add(new SubTrail(command, nodeSprite, trailMarkingSprite));
    }

    public void ExtendPattern()
    {
       
    }
}
