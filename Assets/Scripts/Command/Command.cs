using UnityEngine;
using System.Collections;

public abstract class Command {

	public enum AvailableCommands { None, Move, Push };
	public GameObject robot;
	//execute is called by the robot
	public abstract void Execute();
	public bool isFinished = false;
	public float lifeDuration;
	public float lifeTimer;

	public Vector2 targetPosition;
	public int turn;

	public GameObject Robot { get { return robot; } }

	public float LifeDuration { get { return lifeDuration; } }


}

[System.Serializable]
public class SerializableCommand {

	public enum CommandType { None, Move, Push };
	public CommandType type;
	public int robotIndex;
	public Position targetPosition;
	public float lifeDuration;
	public int turn;
	public Position force;
	public Position initialForce;
	public SerializableCommand(int index, Vector2 targetPos, float duration, CommandType t, int turn, Vector2 force, Vector2 initialForce){
		robotIndex = index;
		targetPosition = new Position(targetPos);
		this.force = new Position(force);
		this.initialForce = new Position(initialForce);
		lifeDuration = duration;
		this.turn = turn;
		type = t;
	}
}