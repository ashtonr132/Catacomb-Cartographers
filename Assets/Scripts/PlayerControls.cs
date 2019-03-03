using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private Rigidbody rb;
    internal readonly float playerspeed = 2;
    private GameObject menu;
    private Animator an;

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.GetComponent<Menu>().inGameSettings();
        }
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            var sc = transform.localScale;
            sc.x *= sc.x > 0 ? -1 : 1;
            transform.localScale = sc;
            rb.velocity = -transform.right *playerspeed;
        }
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            var sc = transform.localScale;
            sc.x *= sc.x > 0 ? 1 : -1;
            transform.localScale = sc;
            rb.velocity = transform.right * playerspeed;
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Rotate(Vector3.forward * playerspeed * transform.localScale.x / transform.localScale.magnitude);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Rotate(-Vector3.forward * playerspeed * transform.localScale.x / transform.localScale.magnitude);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            
        }
    }
}
