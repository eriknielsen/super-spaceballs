﻿using UnityEngine;
using System.Collections;
using System;

public class PushCommand : Command {
    public delegate void InstantiateShockWave(GameObject r, Vector2 dir, float force);
    public static event InstantiateShockWave OnInstantiateShockWave;
    float chargeTime;
    public PushCommand(GameObject r, Vector2 target, float lifetime, int turn)
    {
        lifeDuration = lifetime;
        lifeTimer = lifeDuration;
        chargeTime = 0;
        robot = r;
        this.turn = turn;
        Vector3 target3 = new Vector3(target.x, target.y, 0);
        targetPosition = target3 - r.transform.position;
       
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
            //send out shockwave
            if(OnInstantiateShockWave!= null)
            {
                
                OnInstantiateShockWave(robot, targetPosition, chargeTime);
            }
        }
    }
}
