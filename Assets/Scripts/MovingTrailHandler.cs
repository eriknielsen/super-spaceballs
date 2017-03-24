using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrailHandler : MonoBehaviour
{

    private List<GameObject> movingPreviews;
    private string movingPreviewsName = "Moving Previews";
    private List<List<MovingTrail>> robotMovingTrails;
    private List<List<MovingTrail>> ballMovingTrails;

    private MovingTrail latestRobotTrail;
    private MovingTrail latestBallTrail;

    private List<GameObject> robots;

    private void Awake()
    {
        movingPreviews = new List<GameObject>();
        robotMovingTrails = new List<List<MovingTrail>>();
        ballMovingTrails = new List<List<MovingTrail>>();
    }

    public void Initialize(List<GameObject> robots, GameObject ball)
    {
        this.robots = robots;

        for (int i = 0; i < robots.Count; i++)
        {
            movingPreviews.Add(new GameObject());
            movingPreviews[i].name = movingPreviewsName;
            movingPreviews[i].SetActive(false);
            robotMovingTrails.Add(new List<MovingTrail>());
            ballMovingTrails.Add(new List<MovingTrail>());
        }
    }

    public void MakeRobotTrail(Command commmand)
    {
        if (commmand.Robot != null && commmand.Robot.GetComponent<RobotBehaviour>())
        {

        }
    }

    void DisableMovingPreviews()
    {
        for (int i = 0; i < robots.Count; i++)
        {
            movingPreviews[i].SetActive(false);
        }
    }

    public GameObject GetLatestTrailNode(GameObject robot)
    {
        GameObject latestNode = null;

        for(int i = 0; i < robots.Count; i++)
        {
            if(robots[i] == robot)
            {
                latestNode = robot;
            }
        }

        return latestNode;
    }
}
