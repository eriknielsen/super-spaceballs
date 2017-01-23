using UnityEngine;
using System.Collections;

public class testSleep : MonoBehaviour {

    float timeLeft = 300;
    Rigidbody2D rb2d;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = new Vector2(2, 0);
    }

	void Update ()
    {
        if(Input.GetButtonDown("Jump") && rb2d.IsSleeping() == false)
        {
            rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else if (Input.GetButtonDown("Jump") && rb2d.IsSleeping() == true)
        {
            rb2d.constraints = RigidbodyConstraints2D.None;
        }
    }
}
