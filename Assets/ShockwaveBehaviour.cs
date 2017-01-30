using UnityEngine;
using System.Collections;

public class ShockwaveBehaviour : MonoBehaviour {

    [HideInInspector]
    public float extraChargeForce;
    //public Vector2 direction;
    public float standardPushForce;
    public float currentPushForce;
    float lifeTimer;
    [HideInInspector]
    public Vector2 direction;

    Rigidbody2D rb;
	// Use this for initialization
    
	void Awake () {
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

        if (lifeTimer >= 0)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity);
            lifeTimer -= Time.fixedDeltaTime;
            currentPushForce -= Time.fixedDeltaTime;
            
            transform.localScale += new Vector3(0.1f*Time.fixedDeltaTime, 0.1f*Time.fixedDeltaTime, 0f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
