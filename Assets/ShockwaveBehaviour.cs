using UnityEngine;
using System.Collections;

public class ShockwaveBehaviour : MonoBehaviour {

    public float initialPushForce;
    public Vector2 direction;
    public float pushForce;
    float lifeTimer;
    [SerializeField]
    float moveForce;

    Rigidbody2D rb;
	// Use this for initialization
    
	void Awake () {
        rb = GetComponent<Rigidbody2D>();
        //pushForce = initialPushForce;
        lifeTimer = 4f;
	}
	void FixedUpdate()
    {
        if(lifeTimer >= 0)
        { 
            lifeTimer -= Time.fixedDeltaTime;
            pushForce -= Time.fixedDeltaTime;
            transform.localScale += new Vector3(0.1f*Time.fixedDeltaTime, 0.1f*Time.fixedDeltaTime, 0f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
