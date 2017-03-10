using UnityEngine;
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
    bool hasStarted = false;
    RobotBehaviour robotScript;
    Rigidbody2D robotRb2d;
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

    public Vector2 StartPosition
    {
        get
        {
            return startPosition;
        }
    }

  

    public MoveCommand(GameObject r, MoveCommand moveCommand)
    {
        robot = moveCommand.Robot;
        robotScript = robot.GetComponent<RobotBehaviour>();
        float speed = robotScript.moveCommandAcceleration;
        lifeDuration = moveCommand.lifeDuration;
        force = moveCommand.Force;
        
        initialForce = moveCommand.InitialForce;
        lifeTimer = moveCommand.LifeDuration;
        targetPosition = moveCommand.targetPosition;
        
        robotRb2d = robot.GetComponent<Rigidbody2D>();
        startPosition = r.transform.position;

    }
    
    public MoveCommand(GameObject r, Vector2 target, float lifetime, int turn)
    {
        float speed = r.GetComponent<RobotBehaviour>().moveCommandAcceleration;
        if (speed <= 0)
        {
            speed = 1f;
        }
        initialForceMagnitude = speed * 7;

        targetPosition = target;
        robot = r;
        lifeDuration = lifetime;
        lifeTimer = lifetime;
        this.turn = turn;
        startPosition = r.transform.position;
    
     
        force = CaluculateForce(speed);
        initialForce = CaluculateForce(initialForceMagnitude);

        robotScript = robot.GetComponent<RobotBehaviour>();
        robotRb2d = robot.GetComponent<Rigidbody2D>();
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
        
        //on the first execute, do this
        if (hasStarted == false)
        {
           robotScript.OnAccelerate();
            robotRb2d.AddForce(InitialForce);
            hasStarted = true;
            if(robotScript.isPreview == false){
                 //Debug.Log("starting at x: " + startPosition.x + " y: " + startPosition.y);
            }
        


        }
        //robot.transform.rotation = Quaternion.Lerp(robot.transform.rotation, Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg), Time.deltaTime);
        if(lifeTimer > 0)
        {
            robotScript.UpdateAnimationAngle(force.y, force.x);
            robotRb2d.AddForce(force);
          

            lifeTimer -= Time.fixedDeltaTime;
            if(robotScript.isPreview == false){
                //Debug.Log(force.x);
            }
            
        }
        else
        {
            
            isFinished = true;
            //if ending, call deaccelerate on the robot
            robotScript.OnDeaccelerate();
        }
        
    }


}
