using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the Laser Sight for the player's aim
/// taken and modified from http://wiki.unity3d.com/index.php/Trajectory_Simulation
/// </summary>
public class PreviewMarker : MonoBehaviour {

	// Number of segments to calculate - more gives a smoother line
	public int segmentCount = 20;

	public float moveCommandMoveForce = 10;
    // Length scale for each segment
    public float segmentScale = 1;

    // gameobject we're actually pointing at (may be useful for highlighting a target, etc.)
    private Collider _hitObject;
    public Collider hitObject { get { return _hitObject; } }

    float timePassed = 0;

	// Reference to the LineRenderer we will use to display the simulated path
	LineRenderer sightLine;

    void Awake(){
        sightLine = gameObject.GetComponent<LineRenderer>();
    }

	///Simulate the path of a launched ball.r
    void FixedUpdate(){
        //simulatePath();
    }

//	// Reference to the LineRenderer we will use to display the simulated path
//	public LineRenderer sightLine;
//
//	// Reference to a Component that holds information about fire strength, location of cannon, etc.
//	public PlayerFire playerFire;
//
//	// Number of segments to calculate - more gives a smoother line
//	public int segmentCount = 20;
//
//	// Length scale for each segment
//	public float segmentScale = 1;
//
//	// gameobject we're actually pointing at (may be useful for highlighting a target, etc.)
//	private Collider _hitObject;
//	public Collider hitObject { get { return _hitObject; } }
//
//	/// Simulate the path of a launched ball.
//	/// Slight errors are inherent in the numerical method used.
//	void FixedUpdate(){
//		Vector3[] segments = new Vector3[segmentCount];
//
//		// The first line point is wherever the player's cannon, etc is
//		segments[0] = playerFire.transform.position;
//
//		// The initial velocity
//		Vector3 segVelocity = playerFire.transform.up * playerFire.fireStrength * Time.deltaTime;
//
//		// reset our hit object
//		_hitObject = null;
//
//		for (int i = 1; i < segmentCount; i++){
//			// Time it takes to traverse one segment of length segScale (careful if velocity is zero)
//			float segTime = (segVelocity.sqrMagnitude != 0) ? segmentScale / segVelocity.magnitude : 0;
//
//			// Add velocity from gravity for this segment's timestep
//			segVelocity = segVelocity + Physics.gravity * segTime;
//
//			// Check to see if we're going to hit a physics object
//			RaycastHit hit;
//			if (Physics.Raycast(segments[i - 1], segVelocity, out hit, segmentScale)){
//				// remember who we hit
//				_hitObject = hit.collider;
//
//				// set next position to the position where we hit the physics object
//				segments[i] = segments[i - 1] + segVelocity.normalized * hit.distance;
//				// correct ending velocity, since we didn't actually travel an entire segment
//				segVelocity = segVelocity - Physics.gravity * (segmentScale - hit.distance) / segVelocity.magnitude;
//				// flip the velocity to simulate a bounce
//				segVelocity = Vector3.Reflect(segVelocity, hit.normal);
//
//				/*
//				 * Here you could check if the object hit by the Raycast had some property - was 
//				 * sticky, would cause the ball to explode, or was another ball in the air for 
//				 * instance. You could then end the simulation by setting all further points to 
//				 * this last point and then breaking this for loop.
//				 */
//			}
//			// If our raycast hit no objects, then set the next position to the last one plus v*t
//			else {
//				segments[i] = segments[i - 1] + segVelocity * segTime;
//			}
//		}
//
//		sightLine.SetColors(startColor, endColor);
//
//		sightLine.SetVertexCount(segmentCount);
//		for (int i = 0; i < segmentCount; i++)
//			sightLine.SetPosition(i, segments[i]);
//	}


//		velocity = previewRobot.GetComponent<Rigidbody2D>().velocity;
//		for (int i = 1; i < physicsSteps; i++){
//			velocity += previewRobot.GetComponent<RobotBehaviour>().moveCommandAcceleration * Time.fixedDeltaTime;
//			RaycastHit hit;    
//			if(Physics.Linecast(previousLocation, (previousLocation + (velocity * Time.fixedDeltaTime)), out hit)){
//				velocity = Vector3.Reflect(velocity * bounceDamping, hit.normal);
//				previousLocation = hit.point;
//			}
//			previousLocation += velocity * Time.fixedDeltaTime;
//		}


    /// <summary>
    /// takes the balls position and velocity and draws a line in the direction
    /// it is moving.
    /// The ball script enables and unenables this line renderer!!!!
    /// </summary>
    /// <param name="initPosition"></param>
    /// <param name="currentVelocity"></param>
   public void showBallDirection(Vector2 initPosition, Vector2 currentVelocity, LineRenderer lr)
   {
		List<Vector3> points = new List<Vector3>();
     
        //add the ball's current position
        points.Add(initPosition);

        //add the ball's "next" position
        Vector3 nextPosition = initPosition;

        //show the ball's position after one second

        currentVelocity = currentVelocity * 0.5f;

        nextPosition.x += currentVelocity.x;
        nextPosition.y += currentVelocity.y;
       
        points.Add(nextPosition);
       
       for(int i = 0; i < points.Count;i++)
        {
            points[i] = new Vector3(points[i].x, points[i].y, -2);
        }
      
    

        lr.numPositions = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            lr.SetPosition(i, points[i]);
        }
    }

    public void simulatepath2(Vector2 initPosition, Vector2 robotPrevVelocity, float time, Vector2 mousePosition)
    {
		List<Vector2> points = new List<Vector2>();
        //få rätt riktning mot musen
        Vector2 velocity = robotPrevVelocity;
        //riktning
        Vector2 heading = mousePosition - initPosition;

        Vector2 robotPosition = initPosition;
        //https://docs.unity3d.com/Manual/DirectionDistanceFromOneObjectToAnother.html
        
        Vector2  direction = heading / heading.magnitude;
        
        int pointCount = (int)(time*10);
        points.Add(robotPosition);

        //movecommandorce är global
        float accelerationForce = moveCommandMoveForce / Time.deltaTime ;
        Debug.Log("initrobot x was: " + robotPosition.x);
        while(pointCount > 0)
        {

            accelerationForce = moveCommandMoveForce * Time.deltaTime;
            robotPosition.x = robotPosition.x + velocity.x + ((accelerationForce / 2) * direction).x;
            robotPosition.y = robotPosition.y + velocity.y + ((accelerationForce / 2) * direction).y;
            velocity.x = velocity.x + (accelerationForce * direction).x;
            velocity.y = velocity.y + (accelerationForce * direction).y;
            
            Vector2 newPoint = new Vector2(robotPosition.x, robotPosition.y);
           
            //raycast between newpoint and previous point to see if anything is in the way
            
            points.Add(newPoint);
            pointCount--;
        }

        // Set the colour of our path to the colour of the next ball
        Color startColor = Color.blue;
        Color endColor = Color.green;

        sightLine.startColor = startColor;
        sightLine.endColor = endColor;

        sightLine.numPositions = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            sightLine.SetPosition(i, points[i]);
        }
    }
}
