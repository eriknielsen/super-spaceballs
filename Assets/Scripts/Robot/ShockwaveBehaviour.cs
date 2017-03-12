using UnityEngine;
using System.Collections;
using System;

public class ShockwaveBehaviour : MonoBehaviour {
    
	public float intendedLifetime;

	[SerializeField]
	private float pushForce;
    [SerializeField]
    private float moveForce;
    [SerializeField]
    private float xScaling;
    [SerializeField]
    private float yScaling;
    [SerializeField]
    private float xPushForceLoss;
    [SerializeField]
    private float yPushForceLoss;

    [SerializeField]
    GameObject awakeSound;
    private Vector2 velocity;
	private Vector2 pushVector;
    private Vector2 moveVector;
    private Rigidbody2D rb2dCompontent;
	private float remainingLifeTime;
	private bool shouldUpdate;
	private GameObject shockwaveUser;
    float realRotation;

	float chargeTime;
    public static ShockwaveBehaviour InstantiateShockWave(ShockwaveBehaviour shockWave) {
        return Instantiate(shockWave);
    }

    void OnValidate() {
        if (intendedLifetime <= 0) {
            Debug.LogError("intended lifetime in shockwave is zero or less, fix!");

            remainingLifeTime = 0;
        }
        if (pushForce < 0) {
            pushForce = 0;
        }
    }

    public void Initialize(Vector2 inputVelocity, float chargeTime, GameObject shockwaveUser) {
        
        this.shockwaveUser = shockwaveUser;
        velocity = inputVelocity;
        moveVector = velocity.normalized * moveForce;
		this.chargeTime = chargeTime;
        pushVector = velocity.normalized * (pushForce * (chargeTime));
        enabled = true;
    }

    void Awake() {
        remainingLifeTime = intendedLifetime;
        shouldUpdate = true;
        if (GetComponent<Rigidbody2D>() == null)
        {
            gameObject.AddComponent<Rigidbody2D>();
        }
        rb2dCompontent = GetComponent<Rigidbody2D>();
        
       
    }
    void Start()
    {
        AudioManager.instance.PlayAudio(awakeSound,false,gameObject);
        
        rb2dCompontent.AddForce(moveVector);
        Vector3 diff = (Vector3)moveVector - transform.position; 

        float rot_z = (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
        rb2dCompontent.rotation = rot_z;
        realRotation = rot_z;
       
        
      }

   void OnPause()
    {        
        Destroy(gameObject);
    }


    void Update() {
       
        if (shouldUpdate) {
            if (remainingLifeTime >= 0) {
                 remainingLifeTime -= Time.deltaTime;
               
                transform.localScale += new Vector3(xScaling*Time.deltaTime , yScaling * Time.deltaTime, 0 );
                if(rb2dCompontent.rotation != realRotation)
                    rb2dCompontent.rotation = realRotation;
				pushVector -= new Vector2(pushVector.normalized.x * xPushForceLoss * chargeTime * Time.deltaTime , pushVector.normalized.y * yPushForceLoss * chargeTime* Time.deltaTime);


            }
            else {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collidingObject) {
        GameObject root = collidingObject.transform.root.gameObject;
        if (root != null && root.GetComponent<Rigidbody2D>() && root != shockwaveUser)
        {
            root.GetComponent<Rigidbody2D>().AddForce(pushVector);
        }
    }
}
