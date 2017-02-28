﻿using UnityEngine;
using System.Collections;

public class MoveCommand : Command
{
    float forceMagnitude, initialForceMagnitude;
    float initialForceTime;
    
    Vector2 force;
    Vector2 initialForce;
    float angle;
    Vector2 startPosition;
    Vector2 startSpeed;

    public Vector2 Force
    {
        get
        {
            return force;
        }
        set
        {
            force = value;
        }
    }

    public Vector2 InitialForce
    {
        get { return initialForce; }
        set { initialForce = value; }
    }

    public float LifeDuration
    {
        get
        {
            return lifeDuration;
        }
    }

    public GameObject Robot
    {
        get
        {
            return robot;
        }
    }

    public Vector2 StartPosition
    {
        get
        {
            return startPosition;
        }
    }

    public Vector2 StartSpeed
    {
        get
        {
            return startSpeed;
        }
    }

    public MoveCommand(GameObject r, MoveCommand moveCommand)
    {
        robot = r;
        lifeDuration = moveCommand.lifeDuration;
        force = moveCommand.Force;
        initialForce = moveCommand.InitialForce;
        lifeTimer = moveCommand.LifeDuration;
    }
    
    public MoveCommand(GameObject r, Vector2 target, float lifetime, int turn)
    {
        forceMagnitude = 1f;
        initialForceMagnitude = forceMagnitude * 7;
        initialForceTime = lifetime - lifetime / 4;
        targetPosition = target;
        robot = r;
        lifeDuration = lifetime;
        lifeTimer = lifetime;
        this.turn = turn;
        startPosition = r.transform.position;
        startSpeed = r.GetComponent<Rigidbody2D>().velocity;

        force = CaluculateForce(forceMagnitude);
        initialForce = CaluculateForce(initialForceMagnitude);
    }

    Vector2 CaluculateForce(float forceMagnitude)
    {
        Vector2 positionDifference = targetPosition - startPosition;
        angle = Mathf.Atan2(positionDifference.y, positionDifference.x);
        if (angle < 0)
        {
            angle = 2 * Mathf.PI + angle;
        }
        float yForce = Mathf.Sin(angle) * forceMagnitude;
        float xForce = Mathf.Cos(angle) * forceMagnitude;
        return new Vector2(xForce, yForce);
    }

    public override void Execute()
    {
        //robot.transform.rotation = Quaternion.Lerp(robot.transform.rotation, Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg), Time.deltaTime);
        if(lifeTimer > 0)
        {
            if(lifeTimer > initialForceTime)
            {
                robot.GetComponent<Rigidbody2D>().AddForce(InitialForce);
            }
            else
            {
                robot.GetComponent<Rigidbody2D>().AddForce(force); ;
            }
            lifeTimer -= Time.deltaTime;
        }
        else
        {
            isFinished = true;
        }
        
    }

    public IEnumerator FinishedCoroutine()
    {
        yield return new WaitForSeconds(lifeDuration);
        isFinished = true;
    }
}
