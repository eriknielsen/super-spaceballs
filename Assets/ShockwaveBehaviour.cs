using UnityEngine;
using System.Collections;
using System;

public class ShockwaveBehaviour : IEntity
{
    [SerializeField]
    float lifeTime;
    [SerializeField]
    float pushForce;

    Vector2 velocity;
    Vector2 pushVector;
    Rigidbody2D rb2dCompontent;

    bool shouldUpdate;

    public static ShockwaveBehaviour InstantiateShockWave(ShockwaveBehaviour shockWave)
    {
        return Instantiate(shockWave);
    }

    void OnValidate()
    {
        if(lifeTime < 0)
        {
            lifeTime = 0;
        }
        if (pushForce < 0)
        {
            pushForce = 0;
        }
    }

    public void Initialize(Vector2 velocity)
    {
        this.velocity = velocity;
        pushVector = velocity.normalized * pushForce;
        enabled = true;
    }

    void Awake()
    {
        shouldUpdate = true;
        if(GetComponent<Rigidbody2D>() == null)
        {
            gameObject.AddComponent<Rigidbody2D>();
        }
        rb2dCompontent = GetComponent<Rigidbody2D>();
        enabled = false;
    }
    void FixedUpdate()
    {
        if (shouldUpdate)
        {
            if (lifeTime >= 0)
            {
                transform.rotation = Quaternion.LookRotation(rb2dCompontent.velocity);
                lifeTime -= Time.fixedDeltaTime;
                rb2dCompontent.velocity = velocity;
                transform.localScale += new Vector3(0.1f * Time.fixedDeltaTime, 0.1f * Time.fixedDeltaTime);
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
        rb2dCompontent.velocity = Vector2.zero;
    }

    public override void EnterPlay()
    {
        if (rb2dCompontent != null)
        {
            shouldUpdate = true;
        }

    }
    public override bool IsFinished()
    {
        return gameObject == null;
    }

    void OnTriggerStay2D(Collider2D collidingObject)
    {
        if(collidingObject.GetComponent<Rigidbody2D>() != null)
        {
            collidingObject.GetComponent<Rigidbody2D>().AddForce(pushVector);
        }
    }
}
