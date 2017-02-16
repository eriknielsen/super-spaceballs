using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

class TrailUpdater : MonoBehaviour
{
    bool isInitialized = false;
    bool isFinished = false;
    Sprite nodeSprite, trailMarkingSprite;
    float nodeWidth = 1.0f, trailMarkingWidth = 0.5f;
    Vector3 previousLocation;
    float distanceBetweenMarkings = 1.0f;
    GameObject node;
    List<GameObject> trailMarkings;
    MoveCommand moveCommand;
    float timeDuration;
    float elapsedTime = 0;

    void Awake()
    {
        nodeSprite = Resources.Load<Sprite>("Sprites/fotball2");
        trailMarkingSprite = Resources.Load<Sprite>("Sprites/fotball2");
        trailMarkings = new List<GameObject>();
    }

    public void Initialize(MoveCommand moveCommand, float lifeTime, Vector2 currentVelocity)
    {
        if (moveCommand.robot != null)
        {
            node = Instantiate(moveCommand.robot, transform) as GameObject;
            foreach (Transform c in node.transform)
            {
                c.gameObject.layer = LayerMask.NameToLayer("No Collision With Robot");
            }

            this.moveCommand = new MoveCommand(node, moveCommand);
            node.GetComponent<RobotBehaviour>().Commands.Add(this.moveCommand);
            node.GetComponent<RobotBehaviour>().CurrentState.EnterPlayState();
            node.name = "Node";
            node.GetComponent<Rigidbody2D>().velocity = currentVelocity;
            timeDuration = lifeTime;
            node.GetComponent<RobotBehaviour>().CurrentState.EnterPlayState();
            previousLocation = node.transform.position;
            nodeSprite = Resources.Load<Sprite>("Sprites/fotball2");
            trailMarkingSprite = Resources.Load<Sprite>("Sprites/fotball2");
            if (node.GetComponent<SpriteRenderer>() == null)
            {
                node.AddComponent<SpriteRenderer>();
            }
            node.GetComponent<SpriteRenderer>().sprite = nodeSprite;
            node.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.5f);

            Time.timeScale = 100;
            isInitialized = true;
        }
        else
        {
            UnityEngine.Debug.Log("The moving object reference passed to the MovingTrail-script is null, so no trail is created.");
        }
    }

    void FixedUpdate()
    {
        MakeTrail();
    }


    void MakeTrail()
    {
        if (isInitialized && !isFinished)
        {
            if (node != null)
            {
                float sizeFactorTrailMarking = trailMarkingWidth / trailMarkingSprite.bounds.size.x;
                if (elapsedTime < timeDuration && !isFinished)
                {
                    float deltaDistance = Vector3.Distance(node.transform.position, previousLocation);
                    if (deltaDistance > distanceBetweenMarkings)
                    {
                        GameObject trailMarking = new GameObject();
                        trailMarking.transform.SetParent(transform);
                        trailMarking.name = "Trailmarking";
                        trailMarking.AddComponent<SpriteRenderer>();
                        trailMarking.GetComponent<SpriteRenderer>().sprite = trailMarkingSprite;
                        trailMarkings.Add(trailMarking);
                        previousLocation = node.transform.position;
                        trailMarking.transform.position = previousLocation;
                        sizeFactorTrailMarking = trailMarkingWidth / trailMarkingSprite.bounds.size.x;
                        trailMarking.transform.localScale = new Vector3(sizeFactorTrailMarking, sizeFactorTrailMarking);
                    }
                    elapsedTime += Time.fixedDeltaTime;
                }
                else
                {
                    node.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0);
                    node.GetComponent<RobotBehaviour>().CurrentState.EnterPauseState();
                    isFinished = true;
                    Time.timeScale = 1.0f;
                }

            }
            else
            {
                UnityEngine.Debug.Log("The node is soooo null");
            }
        }
    }
}

public class MovingTrail
{
    TrailUpdater trailUpdater;

    public MovingTrail(MoveCommand moveCommand, float timeDuration, Vector2 currentVelocity)
    {
        GameObject g = new GameObject();
        g.AddComponent<TrailUpdater>();
        g.name = "MovingTrail";
        trailUpdater = g.GetComponent<TrailUpdater>();
        trailUpdater.Initialize(moveCommand, timeDuration, currentVelocity);
    }

    public GameObject TrailGameObject
    {
        get { return trailUpdater.gameObject;  }
    }
}
