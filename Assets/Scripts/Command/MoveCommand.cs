using UnityEngine;
using System.Collections;

public class MoveCommand : Command {

  
    float force;
    float commandLifetime;
    public MoveCommand(GameObject r, Vector2 target, float lifetime)
    {
        force = 1f;
        targetPosition = target;
        robot = r;
        commandLifetime = lifetime;
    }
	public override void Execute()
    {
        Debug.Log(targetPosition.x);
        //move towards target
        robot.GetComponent<Rigidbody2D>().AddForce(force*targetPosition.normalized);
    }
    public override IEnumerator FinishedCoroutine()
    {
        yield return new WaitForSeconds(commandLifetime);
        isFinished = true;

        Debug.Log("should move onto next command");
    }
}
