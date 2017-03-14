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
    float trailMarkingWidth = 0.5f;
    Vector3 previousLocation;
    float distanceBetweenMarkings = 1.0f;
    public GameObject node;
    SpriteRenderer nodeSpriteRenderer;
    List<GameObject> trailMarkings;
    Command command;
    Type commandType;
    Vector3 startPosition;
    float timeDuration;
    float elapsedTime = 0;
    CommandSymbols commandSymbols;

    void Awake()
    {
        nodeSprite = Resources.Load<Sprite>("Sprites/fotball2");
        trailMarkingSprite = Resources.Load<Sprite>("Sprites/fotball2");
        trailMarkings = new List<GameObject>();
        commandSymbols = FindObjectOfType<CommandSymbols>();
    }

    public void Initialize(Command command, float lifeTime, Vector2 currentVelocity)
    {
        UnityEngine.Debug.Log(command.GetType());
        if (command != null && command.robot != null)
        {
            node = Instantiate(command.robot, transform) as GameObject;
            startPosition = node.transform.position;

            commandType = command.GetType();
            if (commandType == typeof(MoveCommand))
            {
                this.command = new MoveCommand(node, command as MoveCommand);
            }
            else{
                this.command = command;
            }

            foreach (Transform c in node.transform)
            {
                c.gameObject.layer = LayerMask.NameToLayer("No Collision With Robot");
            }
            node.gameObject.layer = LayerMask.NameToLayer("No Collision With Robot");
            nodeSpriteRenderer = node.GetComponent<SpriteRenderer>();
            if(nodeSpriteRenderer != null)
            {
                nodeSpriteRenderer.enabled = false;
            }

            if (node.GetComponent<RobotBehaviour>() == true)
            {
                node.GetComponent<RobotBehaviour>().isPreview = true;
                node.GetComponent<RobotBehaviour>().Commands.Add(this.command);
                node.GetComponent<RobotBehaviour>().CurrentState.EnterPlayState();
            }

            node.name = "Node";
            node.GetComponent<Rigidbody2D>().velocity = currentVelocity;
            timeDuration = lifeTime;
            previousLocation = node.transform.position;
            trailMarkingSprite = Resources.Load<Sprite>("Sprites/Various Sprites/Sheet_runt_fält (1)");
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
                        //nodeSpriteRenderer.enabled = true;
                    }
                    for(int i = 0; i < trailMarkings.Count; i++)
                    {
                        trailMarkings[i].GetComponent<SpriteRenderer>().enabled = true;
                    }
                    if (node.GetComponent<RobotBehaviour>() != null)
                    {
                        node.GetComponent<RobotBehaviour>().CurrentState.EnterPauseState();
                    }
                    if(commandSymbols != null)
                    {
                        GameObject commandSymbol = new GameObject();
                        commandSymbol.AddComponent<SpriteRenderer>();
                        commandSymbol.GetComponent<SpriteRenderer>().sprite = commandSymbols.GetSymbolSprite(commandType);
                        commandSymbol.transform.position = node.transform.position;
                        commandSymbol.transform.parent = transform;
                        commandSymbol.GetComponent<SpriteRenderer>().sortingOrder = node.GetComponent<SpriteRenderer>().sortingOrder + 1;
                    }
                    isFinished = true;
                    Time.timeScale = 1.0f;
                    UnityEngine.Debug.Log(command);
                    if(command.GetType() == typeof(PushCommand)){

                        
                        if(trailMarkings.Count > 0){
                            GameObject.Find("ShockwaveCone").GetComponent<ShockwaveConeScript>().SetConePosition(trailMarkings.Last().transform.position);
                        }
                        else{
                            GameObject.Find("ShockwaveCone").GetComponent<ShockwaveConeScript>().SetConePosition(Vector2.zero);
                        }
                    }
                }

            }
            else
            {
                UnityEngine.Debug.Log("The node is soooo null");
            }
        }
    }

    public void DestroyTrail()
    {
        Destroy(gameObject);
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

    public void DestroyTrail()
    {
        trailUpdater.DestroyTrail();
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
