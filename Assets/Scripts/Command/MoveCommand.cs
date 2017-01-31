﻿using UnityEngine;
using System.Collections;

public class MoveCommand : Command
{
    float force;
    float lifeDuration;
    Vector2 resultingForce;
    float angle;
    float intialForceTimeLeft;
    Vector2 startPosition;
    Vector2 startSpeed;

    public Vector2 ResultingForce
    {
        get
        {
            return resultingForce;
        }
        set
        {
            resultingForce = value;
        }
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



    //public MoveCommand(GameObject r, Vector2 target, float lifetime, int turn)
    //{
    //    force = 3f;
    //    targetPosition = target;
    //    robot = r;
    //    lifeDuration = lifetime;
    //    this.turn = turn;
    //    intialForceTimeLeft = lifetime;
    //}
    //public override void Execute()
    //{

    //    Vector2 positionDifference = targetPosition - (Vector2)robot.transform.position;
    //    angle = Mathf.Atan2(positionDifference.y, positionDifference.x);
    //    if (angle < 0)
    //    {
    //        angle = 2 * Mathf.PI + angle;
    //    }
    //    float yForce = Mathf.Sin(angle) * force;
    //    float xForce = Mathf.Cos(angle) * force;

    //    resultingForce = new Vector2(xForce, yForce);

    //    if (intialForceTimeLeft > 0)
    //    {
    //        Vector2 initialForce = resultingForce * 2;
    //        robot.transform.rotation = Quaternion.Lerp(robot.transform.rotation, Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg), Time.deltaTime);
    //        robot.GetComponent<Rigidbody2D>().AddForce(initialForce);
    //        intialForceTimeLeft -= Time.fixedDeltaTime;
    //    }
    //    else
    //    {
    //        robot.transform.rotation = Quaternion.Lerp(robot.transform.rotation, Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg), Time.deltaTime);
    //        robot.GetComponent<Rigidbody2D>().AddForce(resultingForce);
    //    }

    //}

    public MoveCommand(GameObject r, MoveCommand moveCommand)
    {
        robot = r;
        lifeDuration = moveCommand.lifeDuration;
        resultingForce = moveCommand.ResultingForce;
    }
    
    public MoveCommand(GameObject r, Vector2 target, float lifetime, int turn)
    {
        force = 3f;
        targetPosition = target;
        robot = r;
        lifeDuration = lifetime;
        this.turn = turn;
        intialForceTimeLeft = lifetime;
        startPosition = r.transform.position;
        startSpeed = r.GetComponent<Rigidbody2D>().velocity;

        resultingForce = CaluculateForce();
    }

    Vector2 CaluculateForce()
    {
        Vector2 positionDifference = targetPosition - startPosition;
        angle = Mathf.Atan2(positionDifference.y, positionDifference.x);
        if (angle < 0)
        {
            angle = 2 * Mathf.PI + angle;
        }
        float yForce = Mathf.Sin(angle) * force;
        float xForce = Mathf.Cos(angle) * force;
        return new Vector2(xForce, yForce);
    }

    public override void Execute()
    {
        //robot.transform.rotation = Quaternion.Lerp(robot.transform.rotation, Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg), Time.deltaTime);
        robot.GetComponent<Rigidbody2D>().AddForce(resultingForce);
    }

    public override IEnumerator FinishedCoroutine()
    {
        yield return new WaitForSeconds(lifeDuration);
        isFinished = true;
    }
}
