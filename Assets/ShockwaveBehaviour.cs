using UnityEngine;
using System.Collections;
using System;

public class ShockwaveBehaviour : Entity {

    [HideInInspector]
    public float extraChargeForce;
    //public Vector2 direction;
    public float standardPushForce;
    public float currentPushForce;
    float lifeTimer;
    [HideInInspector]
    public Vector2 direction;

    Rigidbody2D rb;

    Vector2 prevVelocity;
    bool shouldUpdate;
    Vector2 zeroVector;
	void Awake () {
        zeroVector = Vector2.zero;
        shouldUpdate = true;
        rb = GetComponent<Rigidbody2D>();
        currentPushForce = extraChargeForce + standardPushForce;
        //pushForce = initialPushForce;
        lifeTimer = 4f;
	}
    public void Start()
    {
        
        rb.AddForce(currentPushForce * direction);
        
    }
	void FixedUpdate()
    {
        if (shouldUpdate)
        {
            if (lifeTimer >= 0)
            {
                transform.rotation = Quaternion.LookRotation(rb.velocity);
                lifeTimer -= Time.fixedDeltaTime;
                currentPushForce -= Time.fixedDeltaTime;

                transform.localScale += new Vector3(0.1f * Time.fixedDeltaTime, 0.1f * Time.fixedDeltaTime, 0f);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public override void EnterPause()
    { 
        shouldUpdate = false;
        prevVelocity = rb.velocity;
        rb.velocity = zeroVector;
    }

    public override void EnterPlay()
    {
        if(rb != null)
        {
            rb.velocity = prevVelocity;
            shouldUpdate = true;
        }
        
    }
    public override bool IsFinished()
    {
        return gameObject == null;
    }
}
