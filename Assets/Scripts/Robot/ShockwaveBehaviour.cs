using UnityEngine;
using System.Collections;
using System;

public class ShockwaveBehaviour : MonoBehaviour {
    
	public float intendedLifetime;

	[SerializeField]
	private float pushForce;
    [SerializeField]
	private float lifeTime;

	private Vector2 velocity;
	private Vector2 pushVector;
	private Rigidbody2D rb2dCompontent;
	private float remainingLifeTime;
	private bool shouldUpdate;
	private GameObject shockwaveUser;
    
    public static ShockwaveBehaviour InstantiateShockWave(ShockwaveBehaviour shockWave) {
        return Instantiate(shockWave);
    }

    void OnValidate() {
        if (lifeTime < 0) {
            intendedLifetime = lifeTime;

            remainingLifeTime = 0;
        }
        if (pushForce < 0) {
            pushForce = 0;
        }
    }

    public void Initialize(Vector2 velocity, float chargeTime, GameObject shockwaveUser) {
        
        this.shockwaveUser = shockwaveUser;
        this.velocity = velocity;
        pushVector = velocity.normalized * (pushForce * chargeTime);
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
        enabled = false;
    }

    void FixedUpdate() {
        if (shouldUpdate) {
            if (remainingLifeTime >= 0) {
                transform.rotation = Quaternion.LookRotation(rb2dCompontent.velocity);
                remainingLifeTime -= Time.fixedDeltaTime;
                rb2dCompontent.velocity = velocity;
                transform.localScale += new Vector3(0.1f * Time.fixedDeltaTime, 0.1f * Time.fixedDeltaTime);
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
