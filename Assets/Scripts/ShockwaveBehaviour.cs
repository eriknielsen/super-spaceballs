﻿using UnityEngine;
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
    float remainingLifeTime;

    bool shouldUpdate;
    GameObject shockwaveUser;

    public static ShockwaveBehaviour InstantiateShockWave(ShockwaveBehaviour shockWave)
    {
        return Instantiate(shockWave);
    }

    void OnValidate()
    {
        if (lifeTime < 0)
        {
            remainingLifeTime = 0;
        }
        if (pushForce < 0)
        {
            pushForce = 0;
        }
    }

    public void Initialize(Vector2 velocity, GameObject shockwaveUser)
    {
        this.shockwaveUser = shockwaveUser;
        this.velocity = velocity;
        pushVector = velocity.normalized * pushForce;
        enabled = true;
    }

    void Awake()
    {
        remainingLifeTime = lifeTime;
        shouldUpdate = true;
        if (GetComponent<Rigidbody2D>() == null)
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
            if (remainingLifeTime >= 0)
            {
                transform.rotation = Quaternion.LookRotation(rb2dCompontent.velocity);
                remainingLifeTime -= Time.fixedDeltaTime;
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
        GameObject root = collidingObject.transform.root.gameObject;
        
        if (root != null && root.GetComponent<Rigidbody2D>() && root != shockwaveUser)
        {

            root.GetComponent<Rigidbody2D>().AddForce(pushVector);
        }

    }
}
