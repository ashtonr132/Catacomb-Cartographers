using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour {
    Rigidbody rb;
    Animator an;
    // Use this for initialization
    void Start () {
        rb = transform.GetComponent<Rigidbody>();
        an = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        if (rb.velocity.magnitude > 1)
        {
            if (rb.velocity.x > 0.1f)//right
            {
                an.SetInteger("Run", 2);
            }
            else if (rb.velocity.x < -0.1f)//left
            {
                an.SetInteger("Run", 3);
            }
            else if (rb.velocity.z > 0.1f)//up
            {
                an.SetInteger("Run", 1);

            }
            else if (rb.velocity.z < -0.1f)//down
            {
                an.SetInteger("Run", 4);

            }
        }
        else
        {
            an.SetInteger("Run", 0);
        }
    }
}
