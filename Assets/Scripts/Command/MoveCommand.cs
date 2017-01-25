using UnityEngine;
using System.Collections;

public class MoveCommand : Command {

  
    float force;
    float commandLifetime;
    Vector2 resultingForce, initialForce;
    bool afterInitialForce = false;
    float angle;

    public MoveCommand(GameObject r, Vector2 target, float lifetime, int turn)
    {
        force = 3f;
        targetPosition = target;
        robot = r;
        commandLifetime = lifetime;
        this.turn = turn;

        Vector2 positionDifference = targetPosition - (Vector2)robot.transform.position;
        angle = Mathf.Atan(positionDifference.y / positionDifference.x);
        float yForce = Mathf.Sin(angle) * force;
        float xForce = Mathf.Cos(angle) * force;
        //resultingForce = new Vector2(xForce, yForce);
        resultingForce = force * positionDifference.normalized;
        initialForce = resultingForce * 4;
    }
	public override void Execute()
    {
        Vector2 positionDifference = targetPosition - (Vector2)robot.transform.position;
        robot.transform.RotateAround(robot.transform.position, robot.transform.forward, Time.deltaTime * 90);
        if (afterInitialForce)
        {
            robot.GetComponent<Rigidbody2D>().AddForce(resultingForce);
        }
        else
        {
            robot.GetComponent<Rigidbody2D>().AddForce(initialForce);
            Debug.Log("Initial force");
            afterInitialForce = true;
        }

    }
    public override IEnumerator FinishedCoroutine()
    {
        yield return new WaitForSeconds(commandLifetime);
        isFinished = true;
    }
}
