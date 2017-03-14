using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System;

class TrailUpdater : MonoBehaviour
{
	public GameObject previewRobot;
	public bool isFinished = false;
	[SerializeField]
	float nodeOpacity = 0.8f;


	float velocity;
	float nodeWidth = 0.5f;
	float elapsedTime = 0;
	float timeDuration;
	float physicsSteps;
	float bounceDamping;
	float distanceBetweenMarkings = 1.0f;
	bool isInitialized = false;

	Type commandType;
	Command command;
	CommandSymbols commandSymbols;
	Sprite nodeSprite;
	Vector3 startPosition;
	Vector3 previousNodeLocation;
	SpriteRenderer nodeRenderer;
	SpriteRenderer previewRobotRenderer;
	List<GameObject> nodes;

    void Awake()
    {
		nodes = new List<GameObject>();
        commandSymbols = FindObjectOfType<CommandSymbols>();
    }

    public void Initialize(Command command, float lifeTime, Vector2 currentVelocity)
    {
        if (command != null && command.robot != null)
        {
            previewRobot = Instantiate(command.robot, transform) as GameObject;
            startPosition = previewRobot.transform.position;

            commandType = command.GetType();
            if (commandType == typeof(MoveCommand))
            {
                this.command = new MoveCommand(previewRobot, command as MoveCommand);
            }

            foreach (Transform c in previewRobot.transform)
            {
                c.gameObject.layer = LayerMask.NameToLayer("No Collision With Robot");
            }
            previewRobot.gameObject.layer = LayerMask.NameToLayer("No Collision With Robot");
			previewRobotRenderer = previewRobot.GetComponent<SpriteRenderer>();
			if(previewRobotRenderer != null)
            {
				previewRobotRenderer.enabled = false;
            }

            if (previewRobot.GetComponent<RobotBehaviour>() == true)
            {
                previewRobot.GetComponent<RobotBehaviour>().isPreview = true;
                previewRobot.GetComponent<RobotBehaviour>().Commands.Add(this.command);
                previewRobot.GetComponent<RobotBehaviour>().CurrentState.EnterPlayState();
            }

            previewRobot.name = "PreviewRobot";
            previewRobot.GetComponent<Rigidbody2D>().velocity = currentVelocity;
            timeDuration = lifeTime;
            previousNodeLocation = previewRobot.transform.position;
            nodeSprite = Resources.Load<Sprite>("Sprites/Node");

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
        if (isInitialized && !isFinished)
        {
            if (previewRobot != null)
            {
				float sizeFactorTrailMarking = nodeWidth / nodeSprite.bounds.size.x;
                if (elapsedTime < timeDuration && !isFinished)
                {
                    float deltaDistance = Vector3.Distance(previewRobot.transform.position, previousNodeLocation);
                    if (deltaDistance > distanceBetweenMarkings)
                    {
						GameObject node = new GameObject();
                        node.transform.SetParent(transform);
                        node.name = "Marking";
						nodeRenderer = node.AddComponent<SpriteRenderer>();
						nodeRenderer.sprite = nodeSprite;
						nodeRenderer.enabled = false;
						Color originalColor = nodeRenderer.color;
						nodeRenderer.color = new Color(nodeRenderer.color.r, nodeRenderer.color.g, nodeRenderer.color.b, nodeOpacity);

                        nodes.Add(node);
                        previousNodeLocation = previewRobot.transform.position;
						node.transform.position = previousNodeLocation;
                        sizeFactorTrailMarking = nodeWidth / nodeSprite.bounds.size.x;
                        node.transform.localScale = new Vector3(sizeFactorTrailMarking, sizeFactorTrailMarking);
                    }
                    elapsedTime += Time.fixedDeltaTime;
                }
                else
                {
                    for(int i = 0; i < nodes.Count; i++)
                    {
                        nodes[i].GetComponent<SpriteRenderer>().enabled = true;
                    }
                    if (previewRobot.GetComponent<RobotBehaviour>() != null)
                    {
                        previewRobot.GetComponent<RobotBehaviour>().CurrentState.EnterPauseState();
                    }
                    if(commandSymbols != null)
                    {
                        GameObject commandSymbol = new GameObject();
                        commandSymbol.AddComponent<SpriteRenderer>();
                        commandSymbol.GetComponent<SpriteRenderer>().sprite = commandSymbols.GetSymbolSprite(commandType);
                        commandSymbol.transform.position = previewRobot.transform.position;
                        commandSymbol.transform.parent = transform;
                        commandSymbol.GetComponent<SpriteRenderer>().sortingOrder = previewRobot.GetComponent<SpriteRenderer>().sortingOrder + 1;
                    }
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

    public GameObject PreviewRobot
    {
        get { return trailUpdater.previewRobot; }
    }

    public bool IsFinished
    {
        get { return trailUpdater.isFinished; }
    }
}
