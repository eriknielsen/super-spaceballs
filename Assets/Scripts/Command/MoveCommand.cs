using UnityEngine;
using System.Collections;

public class MoveCommand : Command {

  
    float force;
    float commandLifetime;
    Vector2 resultingForce;

    public MoveCommand(GameObject r, Vector2 target, float lifetime, int turn)
    {
        force = 3f;
        targetPosition = target;
        robot = r;
        commandLifetime = lifetime;
        this.turn = turn;

        Vector2 positionDifference = targetPosition - (Vector2)robot.transform.position;
        float angle = Mathf.Atan(positionDifference.y / positionDifference.x);
        float yForce = Mathf.Sin(angle) * force;
        float xForce = Mathf.Cos(angle) * force;
        //resultingForce = new Vector2(xForce, yForce);
        resultingForce = force * positionDifference.normalized;
    }
	public override void Execute()
    {
        //move towards target
        robot.GetComponent<Rigidbody2D>().AddForce(resultingForce);
    }
    public override IEnumerator FinishedCoroutine()
    {
        yield return new WaitForSeconds(commandLifetime);
        isFinished = true;
    }
}
