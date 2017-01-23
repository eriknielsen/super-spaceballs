using UnityEngine;
using System.Collections;

public class MoveCommando : Commando {

    Vector2 targetPosition;
    float force;

    public MoveCommando(GameObject r, Vector2 target)
    {
        force = 50f;
        targetPosition = target;
        robot = r;
    }

	public override void Execute()
    {
        //move towards target
        robot.GetComponent<Rigidbody2D>().AddForce(force*targetPosition.normalized);
        isFinished = true;

    }
}
