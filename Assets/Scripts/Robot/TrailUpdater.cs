using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;

class TrailUpdater : MonoBehaviour {

	public bool finished = false;
	public GameObject previewRobot;


	bool initialized = false;
	float timeDuration;
	float elapsedTime = 0;
	float commandSymbolScale = 1.5f;
	float nodeScale = 0.8f;
	float nodeOpacity = 0.8f;
	float distanceBetweenNodes = 1.0f;
	Vector3 previousLocation;
	Type commandType;
	Command command;
	Sprite nodeSprite, moveSprite, pushSprite;
	SpriteRenderer nodeSR;
	List<GameObject> nodes;

	void Awake(){
		moveSprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Move");
		pushSprite = Resources.Load<Sprite>("Sprites/Buttons/Button_Shockwave");
		nodeSprite = Resources.Load<Sprite>("Sprites/Node");
		nodes = new List<GameObject>();
	}

	public void Initialize(Command command, float lifeTime, Vector2 currentVelocity){
		if (command != null && command.robot != null){
			previewRobot = Instantiate(command.robot, transform);
			previewRobot.name = "PreviewRobot";

			commandType = command.GetType();
			if (commandType == typeof(MoveCommand)){
				this.command = new MoveCommand(previewRobot, command as MoveCommand);
			}
			else {
				this.command = command;
			}

			foreach (Transform c in previewRobot.transform){
				c.gameObject.layer = LayerMask.NameToLayer("No Collision With Robot");
			}
			previewRobot.gameObject.layer = LayerMask.NameToLayer("No Collision With Robot");
			previewRobot.GetComponent<SpriteRenderer>().enabled = false;

			if (previewRobot.GetComponent<RobotBehaviour>() == true){
				previewRobot.GetComponent<RobotBehaviour>().isPreview = true;
				previewRobot.GetComponent<RobotBehaviour>().Commands.Add(this.command);
				previewRobot.GetComponent<RobotBehaviour>().CurrentState.EnterPlayState();
			}

			previewRobot.GetComponent<Rigidbody2D>().velocity = currentVelocity;
			timeDuration = lifeTime;
			previousLocation = previewRobot.transform.position;

			Time.timeScale = 100;
			initialized = true;
		}
		else {
			UnityEngine.Debug.Log("The moving object reference or command passed to the MovingTrail-script is null, so no trail is created.");
		}
	}

	public void DestroyTrail(){
		Destroy(gameObject);
	}

	void FixedUpdate(){
		if (initialized && !finished){
			if (elapsedTime < timeDuration && !finished){
				float deltaDistance = Vector3.Distance(previewRobot.transform.position, previousLocation);

				if (deltaDistance > distanceBetweenNodes){
					CreateNode();
				}
				elapsedTime += Time.fixedDeltaTime;
			}
			else {
				for (int i = 0; i < nodes.Count; i++){
					nodes[i].GetComponent<SpriteRenderer>().enabled = true;
				}
				previewRobot.GetComponent<RobotBehaviour>().CurrentState.EnterPauseState();
				CreateCommandSymbol();

				if (command.GetType() == typeof(PushCommand)){
					if (nodes.Count > 0){
						GameObject.Find("ShockwaveCone").GetComponent<ShockwaveConeScript>().SetConePosition(nodes.Last().transform.position);
					}
					else {
						GameObject.Find("ShockwaveCone").GetComponent<ShockwaveConeScript>().SetConePosition(Vector2.zero);
					}
				}
				finished = true;
				Time.timeScale = 1.0f;
			}
		}
	}
		
	void CreateNode(){
		GameObject node = new GameObject();
		node.name = "Node";
		node.transform.SetParent(transform);
		nodeSR = node.AddComponent<SpriteRenderer>();
		nodeSR.sprite = nodeSprite;
		nodeSR.color = new Color(nodeSR.color.r, nodeSR.color.g, nodeSR.color.b, nodeOpacity); //For transparency
		nodeSR.sortingOrder = previewRobot.GetComponent<SpriteRenderer>().sortingOrder + 1;
		nodeSR.enabled = false;

		previousLocation = previewRobot.transform.position;
		node.transform.position = previousLocation;
		node.transform.localScale = node.transform.localScale * nodeScale;
		nodes.Add(node);
	}

	void CreateCommandSymbol(){
		GameObject commandSymbol = new GameObject();
		SpriteRenderer commandSymbolSR = commandSymbol.AddComponent<SpriteRenderer>();
		if (commandType == typeof(MoveCommand)){
			if (moveSprite != null){
				commandSymbolSR.sprite = moveSprite;
			}
		}
		else if (commandType == typeof(PushCommand)){
			if (moveSprite != null){
				commandSymbolSR.sprite = pushSprite;
			}
		}
		commandSymbol.transform.position = previewRobot.transform.position;
		commandSymbol.transform.parent = transform;
		commandSymbol.transform.localScale = commandSymbol.transform.localScale * commandSymbolScale;
		commandSymbol.GetComponent<SpriteRenderer>().sortingOrder = previewRobot.GetComponent<SpriteRenderer>().sortingOrder + 2;
		nodes.Add(commandSymbol);
	}
}

public class MovingTrail {
	public bool Finished { get { return trailUpdater.finished; } }
	public GameObject Node { get { return trailUpdater.previewRobot; } }
	public GameObject TrailGameObject { get { return trailUpdater.gameObject; }	}

	TrailUpdater trailUpdater;

	public MovingTrail(Command command, float timeDuration, Vector2 currentVelocity){
		GameObject go = new GameObject();
		go.AddComponent<TrailUpdater>();
		go.name = "MovingTrail";
		trailUpdater = go.GetComponent<TrailUpdater>();
		trailUpdater.Initialize(command, timeDuration, currentVelocity);
	}

	public void DestroyTrail(){
		trailUpdater.DestroyTrail();
	}
}