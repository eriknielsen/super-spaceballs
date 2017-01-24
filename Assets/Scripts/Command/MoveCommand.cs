using UnityEngine;
using System.Collections;

public class MoveCommand : Command {

  
    float force;
    float commandLifetime;
    public MoveCommand(GameObject r, Vector2 target, float lifetime, int turn)
    {
        force = 1f;
        targetPosition = target;
        robot = r;
        commandLifetime = lifetime;
        this.turn = turn;
    }
	public override void Execute()
    {

        //move towards target
        robot.GetComponent<Rigidbody2D>().AddForce(force*targetPosition.normalized);
    }
    public override IEnumerator FinishedCoroutine()
    {
        yield return new WaitForSeconds(commandLifetime);
        isFinished = true;
    }
}
