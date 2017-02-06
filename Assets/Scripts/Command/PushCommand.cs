using UnityEngine;
using System.Collections;
using System;

public class PushCommand : Command {

    GameObject shockwavePrefab;

    public delegate void InstantiateShockWave(GameObject r, Vector2 dir, float force);
    public static event InstantiateShockWave OnInstantiateShockWave;

    float chargeTime;
    Vector2 velocity;

    public PushCommand(GameObject r, Vector2 velocity, float lifetime)
    {
        this.velocity = velocity;
        lifeDuration = lifetime;
        lifeTimer = lifeDuration;
        chargeTime = 0;
        robot = r;

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
            shockWave.Initialize(velocity);
            shockWave.transform.position = robot.transform.position;     
        }
    }
}
