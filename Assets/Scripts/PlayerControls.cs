using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private Rigidbody rb;
    internal float playerSpeed = 3;
    private GameObject menu;
    private Animator an;
    internal int currentHealth, projSpeed = 5;
    internal static int maxHealth = 100; 
    [SerializeField]
    GameObject Arrow;

	// Use this for initialization
	void Start ()
    {
        currentHealth = maxHealth;
        menu = GameObject.Find("Menu");
        rb = transform.GetComponent<Rigidbody>();
        an = transform.GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.mousePosition.x > Screen.width/2)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y,transform.localScale.z);
        }
        if (Input.GetKey(KeyCode.Mouse0) && !an.GetBool("Moving"))
        {
            if (an.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle2")
            {
                an.SetTrigger("Attack" + Random.Range(1, 4).ToString());
            }
            else if (an.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle1")
            {
                an.SetTrigger("Draw");
                an.SetBool("AttackMode", true);
            }
        }
        else if (Input.GetKey(KeyCode.Mouse1) && !an.GetBool("Moving"))
        {
            if (an.GetBool("AttackMode"))
            {
                an.SetTrigger("Sheathe");
                an.SetBool("AttackMode", false);
            }
            an.SetTrigger("BowAttack");
        }
        else if (Input.GetKeyDown(KeyCode.Q) && !an.GetBool("Moving"))
        {
            an.SetTrigger("Cast");
        }
        else if (Input.GetKey(KeyCode.Q) && !an.GetBool("Moving"))
        {
            an.SetBool("CastLoop", true);
        }
        else if(Input.GetKeyDown(KeyCode.C) && !an.GetBool("Moving"))
        {
            an.SetTrigger("Use");
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !an.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Slide"))
        {
            if (Random.value > 0.5f)
            {
                an.SetTrigger("Slide");
            }
            else
            {
                an.SetTrigger("Slide2");
            }
            var velo = (Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, 0)).normalized;
            if (!an.GetBool("Moving"))
            {
                an.SetBool("Moving", true);
                velo *= playerSpeed;
            }
            velo.z = velo.y;
            velo.y = 0;
            rb.velocity = (velo / velo.magnitude) * (playerSpeed + Random.Range(5, 8));
        }
        else if (Vector3.Distance(Input.mousePosition, new Vector3(Screen.width / 2, Screen.height / 2, 0)) > 125f)
        {
            if (an.GetBool("AttackMode") == true)
            {
                an.SetBool("AttackMode", false);
                an.SetTrigger("Sheathe");
            }
            else if (!an.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Sheathe") && !an.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Idle2") && !an.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Attack") && !an.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Draw") && !an.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Slide"))
            {
                an.SetBool("Moving", true);
                var velo = (Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, 0)).normalized;
                velo.z = velo.y;
                velo.y = 0;
                rb.velocity = velo * playerSpeed;
                clampSpeed();
            }
        }
        else if (an.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Move") && rb.velocity.magnitude < 0.15f)
        {
            rb.velocity = Vector3.zero;
            an.SetBool("Moving", false);
        }
    }
    internal void HurtPlayer(int damage)
    {
        an.SetTrigger("Hurt");
        if ((currentHealth - damage) > 0)
        {
            currentHealth -= damage;
        }
        else
        {
            an.SetTrigger("Die");
        }
    }
    internal void arrowSpawn()
    {
        GameObject arrow = Instantiate(Arrow, transform.position, Quaternion.Euler(new Vector3(90, 0, 0)), null);
        Physics.IgnoreCollision(transform.GetComponent<BoxCollider>(), arrow.GetComponent<BoxCollider>(), true);
        arrow.GetComponent<Rigidbody>().velocity = transform.right * projSpeed * (transform.localScale.x / transform.localScale.magnitude);
        Destroy(arrow, 10);
    }
    internal void clampSpeed()
    {
        transform.GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Clamp(transform.GetComponent<Rigidbody>().velocity.x, -playerSpeed, playerSpeed), 0, Mathf.Clamp(transform.GetComponent<Rigidbody>().velocity.z, -playerSpeed, playerSpeed));
    }
    internal void checkCastLoop()
    {
        if (!Input.GetKey(KeyCode.Q))
        {
            an.SetBool("CastLoop", false);
        }
    }
}
