using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {
    Rigidbody rb;
    float playerspeed = 2;
	// Use this for initialization
	void Start () {
        rb = transform.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        var hold = rb.velocity;

        if (Input.GetKey(KeyCode.A))
        {
            hold.x = -playerspeed;
            rb.velocity = hold;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            hold.x = playerspeed;
            rb.velocity = hold;
        }
        else
        {
            hold.x = 0;
            rb.velocity = hold;
        }
        if (Input.GetKey(KeyCode.W))
        {
            hold.z = playerspeed;
            rb.velocity = hold;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            hold.z = -playerspeed;
            rb.velocity = hold;
        }
        else
        {
            hold.z = 0;
            rb.velocity = hold;
        }

    }
}
