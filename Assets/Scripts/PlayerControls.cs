using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    Rigidbody rb;
    float playerspeed = 2;
    GameObject menu;
    Animator an;

	// Use this for initialization
	void Start ()
    {
        menu = GameObject.Find("Menu");
        rb = transform.GetComponent<Rigidbody>();
        an = transform.GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(rb.velocity.x) < 0.2f)
        {
            an.SetBool("Moving", false);
            an.SetBool("Idle", true);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.GetComponent<Menu>().inGameSettings();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            an.SetBool("Moving", true);
            an.SetBool("Idle", false);
            var sc = transform.localScale;
            sc.x *= sc.x > 0 ? -1 : 1;
            transform.localScale = sc;
            rb.velocity = -transform.right *playerspeed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            an.SetBool("Moving", true);
            an.SetBool("Idle", false);
            var sc = transform.localScale;
            sc.x *= sc.x > 0 ? 1 : -1;
            transform.localScale = sc;
            rb.velocity = transform.right * playerspeed;
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Rotate(Vector3.forward * playerspeed);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Rotate(-Vector3.forward * playerspeed);
        }
    }
}
