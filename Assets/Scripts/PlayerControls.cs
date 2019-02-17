using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    Rigidbody rb;
    float playerspeed = 2;
    GameObject menu;
	// Use this for initialization
	void Start ()
    {
        menu = GameObject.Find("Menu");
        rb = transform.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        rb.velocity = new Vector3(Input.GetAxis("Horizontal") * playerspeed, 0, Input.GetAxis("Vertical") * playerspeed);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.GetComponent<Menu>().inGameSettings();
        }
    }
}
