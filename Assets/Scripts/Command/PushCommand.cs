using UnityEngine;
using System.Collections;
using System;

public class PushCommand : Command {

    GameObject shockwavePrefab;

    public delegate void InstantiateShockWave(GameObject r, Vector2 dir, float force);
    public static event InstantiateShockWave OnInstantiateShockWave;

    float chargeTime;
    Vector2 velocity;
    float speed = 8f;

    public PushCommand(GameObject robot, Vector2 target, float lifetime, int turn)
    {
        this.turn = turn;
        float angle = AngleBetweenPoints(target, robot.transform.position);

        Vector2 velocity = new Vector2(speed * Mathf.Cos(angle), speed * Mathf.Sin(angle));
        
        targetPosition = target;
        this.velocity = velocity;
        lifeDuration = lifetime;
        lifeTimer = lifeDuration;
        chargeTime = 0.1f;
        base.robot = robot;

        shockwavePrefab = Resources.Load("Prefabs/ShockWave") as GameObject;     
    }
    public override void Execute()
    {
        if (lifeTimer >= 0)
        {    
            chargeTime = chargeTime + Time.deltaTime;
            lifeTimer -= Time.deltaTime;
        }
        else
        {
            isFinished = true;
        }
        if (isFinished)
        {
            ShockwaveBehaviour shockWave = ShockwaveBehaviour.InstantiateShockWave(shockwavePrefab.GetComponent<ShockwaveBehaviour>());
            shockWave.Initialize(velocity, chargeTime, robot);
            shockWave.transform.position = robot.transform.position;     
        }

    }
    float AngleBetweenPoints(Vector2 point1, Vector2 point2)
    {
        Vector2 delta = point1 - point2;
        float angle = Mathf.Atan2(delta.y, delta.x);
        if (angle < 0)
        {
            angle = 2 * Mathf.PI + angle;
        }
        return angle;
    }
}
