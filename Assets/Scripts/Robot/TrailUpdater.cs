using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System;

class TrailUpdater : MonoBehaviour
{
    bool isInitialized = false;
    public bool isFinished = false;
    Sprite nodeSprite, trailMarkingSprite;
    float nodeWidth = 1.0f, trailMarkingWidth = 0.5f;
    Vector3 previousLocation;
    float distanceBetweenMarkings = 1.0f;
    public GameObject node;
    SpriteRenderer nodeSpriteRenderer;
    List<GameObject> trailMarkings;
    Command command;
    Vector3 startPosition;
    float timeDuration;
    float elapsedTime = 0;
    void Awake()
    {
        nodeSprite = Resources.Load<Sprite>("Sprites/fotball2");
        trailMarkingSprite = Resources.Load<Sprite>("Sprites/fotball2");
        trailMarkings = new List<GameObject>();
    }

    public void Initialize(Command command, float lifeTime, Vector2 currentVelocity)
    {
        if (command != null && command.robot != null)
        {
            node = Instantiate(command.robot, transform) as GameObject;
            if(node.GetComponent<RobotBehaviour>())
            {
                node.GetComponent<RobotBehaviour>().isPreview = true;
            }

            startPosition = node.transform.position;
            foreach (Transform c in node.transform)
            {
                if (c.gameObject.layer != LayerMask.NameToLayer("Passive Hitbox"))
                {
                    c.gameObject.layer = LayerMask.NameToLayer("No Collision With Robot");
                }
            }
            Type t = command.GetType();
            if (command.GetType() == typeof(MoveCommand))
            {
                this.command = new MoveCommand(node, command as MoveCommand);
            }
            nodeSpriteRenderer = node.GetComponent<SpriteRenderer>();
            if(nodeSpriteRenderer != null)
            {
                nodeSpriteRenderer.enabled = false;
            }

            node.GetComponent<RobotBehaviour>().Commands.Add(this.command);
            node.GetComponent<RobotBehaviour>().CurrentState.EnterPlayState();
            node.name = "Node";
            node.GetComponent<Rigidbody2D>().velocity = currentVelocity;
            timeDuration = lifeTime;
            node.GetComponent<RobotBehaviour>().CurrentState.EnterPlayState();
            previousLocation = node.transform.position;
            trailMarkingSprite = Resources.Load<Sprite>("Sprites/Prototype stuff (remove or rework)/fotball2");
            if (node.GetComponent<SpriteRenderer>() == null)
            {
                node.AddComponent<SpriteRenderer>();
            }
            node.GetComponent<SpriteRenderer>().sprite = nodeSprite;
            float red, green, blue;
            red = node.GetComponent<SpriteRenderer>().color.r;
            green = node.GetComponent<SpriteRenderer>().color.g;
            blue = node.GetComponent<SpriteRenderer>().color.b;
            node.GetComponent<SpriteRenderer>().color = new Color(red, green, blue, 0.5f);

            Time.timeScale = 100;
            isInitialized = true;
        }
        else
        {
            UnityEngine.Debug.Log("The moving object reference or command passed to the MovingTrail-script is null, so no trail is created.");
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
                        trailMarking.GetComponent<SpriteRenderer>().enabled = false;

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
                    if(nodeSpriteRenderer != null && node.transform.position != startPosition)
                    {
                        nodeSpriteRenderer.enabled = true;
                    }
                    for(int i = 0; i < trailMarkings.Count; i++)
                    {
                        trailMarkings[i].GetComponent<SpriteRenderer>().enabled = true;
                    }
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

    public MovingTrail(Command command, float timeDuration, Vector2 currentVelocity)
    {
        GameObject g = new GameObject();
        g.AddComponent<TrailUpdater>();
        g.name = "MovingTrail";
        trailUpdater = g.GetComponent<TrailUpdater>();
        trailUpdater.Initialize(command, timeDuration, currentVelocity);
    }

    public GameObject TrailGameObject
    {
        get { return trailUpdater.gameObject; }
    }

    public GameObject Node
    {
        get { return trailUpdater.node; }
    }

    public bool IsFinished
    {
        get { return trailUpdater.isFinished; }
    }
}
