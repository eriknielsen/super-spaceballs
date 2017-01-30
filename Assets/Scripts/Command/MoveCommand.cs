using UnityEngine;
using System.Collections;

public class MoveCommand : Command
{
    float force;
    
    
    Vector2 resultingForce;
    float angle;
    float intialForceTimeLeft;

    public Vector2 ResultingForce
    {
        get
        {
            Vector2 positionDifference = targetPosition - (Vector2)robot.transform.position;
            angle = Mathf.Atan2(positionDifference.y, positionDifference.x);
            if (angle < 0)
            {
                angle = 2 * Mathf.PI + angle;
            }
            float yForce = Mathf.Sin(angle) * force;
            float xForce = Mathf.Cos(angle) * force;
            resultingForce = new Vector2(xForce, yForce);
            return resultingForce;
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

    public MoveCommand(GameObject r, Vector2 target, float lifetime, int turn)
    {
        force = 3f;
        targetPosition = target;
        robot = r;
        lifeDuration = lifetime;
        lifeTimer = lifetime;
        this.turn = turn;
        intialForceTimeLeft = lifetime;
    }
    public override void Execute()
    {

        if(lifeTimer >= 0)
        {
            Debug.Log("executing");
            lifeTimer -= Time.deltaTime;
            Vector2 positionDifference = targetPosition - (Vector2)robot.transform.position;
            angle = Mathf.Atan2(positionDifference.y, positionDifference.x);
            if (angle < 0)
            {
                angle = 2 * Mathf.PI + angle;
            }
            float yForce = Mathf.Sin(angle) * force;
            float xForce = Mathf.Cos(angle) * force;

            resultingForce = new Vector2(xForce, yForce);

            if (intialForceTimeLeft > 0)
            {
                Vector2 initialForce = resultingForce * 2;
                robot.transform.rotation = Quaternion.Lerp(robot.transform.rotation, Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg), Time.deltaTime);
                robot.GetComponent<Rigidbody2D>().AddForce(initialForce);
                intialForceTimeLeft -= Time.fixedDeltaTime;
            }
            else
            {
                robot.transform.rotation = Quaternion.Lerp(robot.transform.rotation, Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg), Time.deltaTime);
                robot.GetComponent<Rigidbody2D>().AddForce(resultingForce);
            }
        }
        else
        {
            isFinished = true;
        }
       


    }
 
}
