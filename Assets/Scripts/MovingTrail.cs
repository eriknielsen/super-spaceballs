using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class MovingTrail
{
    TrailUpdater trailUpdater;

    public MovingTrail(MoveCommand moveCommand)
    {
        GameObject g = new GameObject();
        g.AddComponent<TrailUpdater>();
        g.name = "TrailUpdater";
        trailUpdater = g.GetComponent<TrailUpdater>();
        trailUpdater.Initialize(moveCommand);
    }

    class TrailUpdater : MonoBehaviour
    {
        bool isFinished = false;
        Sprite nodeSprite, trailMarkingSprite;
        float nodeWidth = 1.0f, trailMarkingWidth = 0.5f;
        Vector3 previousLocation;
        float distanceBetweenMarkings = 1.0f;
        GameObject node;
        List<GameObject> trailMarkings;
        MoveCommand moveCommand;
        float timeDuration;

        void Awake()
        {
            nodeSprite = Resources.Load<Sprite>("Sprites/fotball2");
            trailMarkingSprite = Resources.Load<Sprite>("Sprites/fotball2");
            trailMarkings = new List<GameObject>();
        }

       public void Initialize(MoveCommand moveCommand)
        {
            if (moveCommand.robot != null)
            {
                node = Instantiate(moveCommand.robot) as GameObject;
                this.moveCommand = new MoveCommand(node, moveCommand);
                node.GetComponent<RobotBehaviour>().Commands.Add(this.moveCommand);
                node.GetComponent<RobotBehaviour>().CurrentState.EnterPlayState();
                node.name = "Node";
                node.GetComponent<Rigidbody2D>().velocity = moveCommand.Robot.GetComponent<Rigidbody2D>().velocity;
                timeDuration = moveCommand.LifeDuration;
                StartCoroutine(MakeTrail());
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

                float sizeFactor = nodeWidth / nodeSprite.bounds.size.x;
                node.transform.localScale = new Vector3(sizeFactor, sizeFactor);
            }
            else
            {
                UnityEngine.Debug.Log("The moving object reference passed to the MovingTrail-script is null, so no trail is created.");
            }
        }

        void Update()
        {
            if (isFinished)
            {
                StopCoroutine(MakeTrail());
            }
        }

        IEnumerator MakeTrail()
        {
            if (node != null)
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                float sizeFactorTrailMarking = trailMarkingWidth / trailMarkingSprite.bounds.size.x;
                while (timer.Elapsed.TotalSeconds < timeDuration)
                {
                    float deltaDistance = Vector3.Distance(node.transform.position, previousLocation);
                    if (deltaDistance > distanceBetweenMarkings)
                    {
                        GameObject trailMarking = new GameObject();
                        trailMarking.name = "Trailmarking";
                        trailMarking.AddComponent<SpriteRenderer>();
                        trailMarking.GetComponent<SpriteRenderer>().sprite = trailMarkingSprite;
                        trailMarkings.Add(trailMarking);
                        previousLocation = node.transform.position;
                        trailMarking.transform.position = previousLocation;
                        sizeFactorTrailMarking = trailMarkingWidth / trailMarkingSprite.bounds.size.x;
                        trailMarking.transform.localScale = new Vector3(sizeFactorTrailMarking, sizeFactorTrailMarking);
                    }
                    yield return new WaitForSeconds(0.0001f);
                }
                node.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0);
                node.GetComponent<RobotBehaviour>().CurrentState.EnterPauseState();
                isFinished = true;
            }
            else
            {
                UnityEngine.Debug.Log("The node is soooo null");
            }
        }
    }
}
