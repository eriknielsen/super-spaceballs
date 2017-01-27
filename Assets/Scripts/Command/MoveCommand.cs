using UnityEngine;
using System.Collections;

public class MoveCommand : Command
{


    float force;
    float commandLifetime;
    Vector2 resultingForce;
    float angle;
    float intialForceTimeLeft;

    public MoveCommand(GameObject r, Vector2 target, float lifetime, int turn)
    {
        force = 3f;
        targetPosition = target;
        robot = r;
        commandLifetime = lifetime;
        this.turn = turn;
        intialForceTimeLeft = lifetime;
    }
    public override void Execute()
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
    public override IEnumerator FinishedCoroutine()
    {
        yield return new WaitForSeconds(commandLifetime);
        isFinished = true;
    }
}
