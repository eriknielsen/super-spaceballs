using UnityEngine;
using System.Collections;
using System;

public class ShockwaveBehaviour : MonoBehaviour {
    
	public float intendedLifetime;

	[SerializeField]
	private float pushForce;

    [SerializeField]
    GameObject awakeSound;
    private Vector2 velocity;
	private Vector2 pushVector;
	private Rigidbody2D rb2dCompontent;
	private float remainingLifeTime;
	private bool shouldUpdate;
	private GameObject shockwaveUser;
    float realRotation;
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

    public void Initialize(Vector2 velocity, float chargeTime, GameObject shockwaveUser) {
        
        this.shockwaveUser = shockwaveUser;
        this.velocity = velocity;
        pushVector = velocity.normalized * (pushForce * (1+chargeTime));
        
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

        PlayBehaviour.PreOnPauseGame += OnPause;
    }
    void Start()
    {
        AudioManager.instance.PlayAudio(awakeSound,false,gameObject);
        
        rb2dCompontent.AddForce(pushVector);
        Vector3 diff = (Vector3)pushVector - transform.position; 

        float rot_z = (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
        rb2dCompontent.rotation = rot_z;
        realRotation = rot_z;
      }
   void OnPause()
    {
        
        Destroy(gameObject);
    }
    void OnDestroy()
    {
        PlayBehaviour.PreOnPauseGame -= OnPause;
    }
    void Update() {
       
        if (shouldUpdate) {
            if (remainingLifeTime >= 0) {
                 remainingLifeTime -= Time.deltaTime;
                 //scaling changes rotation??
                transform.localScale += new Vector3(0.5f*Time.deltaTime , 0.5f * Time.deltaTime, 0 );
                if(rb2dCompontent.rotation != realRotation)
                    rb2dCompontent.rotation = realRotation;
            }
            else {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerStay2D(Collider2D collidingObject) {
        GameObject root = collidingObject.transform.root.gameObject;       
        if (root != null && root.GetComponent<Rigidbody2D>() && root != shockwaveUser)
        {
            root.GetComponent<Rigidbody2D>().AddForce(pushVector);
        }
    }
}
