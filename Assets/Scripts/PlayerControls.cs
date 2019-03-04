using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private Rigidbody rb;
    internal float playerSpeed = 2, rotSpeed, buttonCD = 0.5f;
    private GameObject menu;
    private Animator an;
    internal int currentHealth, projSpeed = 5, buttonCount = 0;
    internal static int maxHealth = 100; 
    [SerializeField]
    GameObject Arrow;
    KeyCode lastKey;

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
        if (Input.anyKeyDown)
        {
            if (buttonCD > 0 && buttonCount == 1 && ((Input.GetKeyDown(KeyCode.A) && lastKey == KeyCode.A) || (Input.GetKeyDown(KeyCode.D) && lastKey == KeyCode.D)))
            {
                if (Random.value > 0.5f)
                {
                    an.SetTrigger("Slide2");
                }
                else
                {
                    an.SetTrigger("Slide");
                }
                if (an.GetBool("AttackMode"))
                {
                    an.SetBool("AttackMode", false);
                }
                if (lastKey == KeyCode.A)
                {
                    //transform.GetComponent<Rigidbody>().velocity = -transform.right * playerSpeed * 3;
                    StartCoroutine(rollSpeed(1));
                }
                else
                {
                    //transform.GetComponent<Rigidbody>().velocity = transform.right * playerSpeed * 3;
                    StartCoroutine(rollSpeed(1));
                }
            }
            else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D)))
            {
                buttonCD = 0.5f;
                buttonCount++;
                if (Input.GetKeyDown(KeyCode.A))
                {
                    lastKey = KeyCode.A;
                }
                else
                {
                    lastKey = KeyCode.D;
                }
            }
        }
        if (buttonCD > 0)
        {
            buttonCD -= 1 * Time.deltaTime;
        }
        else
        {
            buttonCount = 0;
        }
            if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.GetComponent<Menu>().inGameSettings();
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) && !an.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Slide"))
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                if (an.GetBool("AttackMode") && !an.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Melee") && !Input.GetKey(KeyCode.Space))
                {
                    an.SetTrigger("Sheathe");
                    an.SetBool("AttackMode", false);
                }
                var sc = transform.localScale;
                int d;
                if (Input.GetKey(KeyCode.A))
                {
                    d = -1;
                }
                else
                {
                    d = 1;
                }
                if (!an.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Melee") && !an.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Bow"))
                {
                    sc.x = Mathf.Abs(sc.x) * d;
                    transform.localScale = sc;
                }
                if (an.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle1" || an.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Move")
                {
                    rb.velocity = d * transform.right * playerSpeed;
                    an.SetBool("Moving", true);
                }
            }
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                if (!an.GetBool("AttackMode"))
                {
                    an.SetBool("Moving", true);
                }
                int d = Input.GetKey(KeyCode.W) ? 1 : -1;
                if (an.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Melee"))
                {
                    rotSpeed = 1.5f;
                }
                else
                {
                    rotSpeed = Mathf.Lerp(rotSpeed, 3, Time.deltaTime);
                }
                transform.Rotate(d * Vector3.forward * playerSpeed * rotSpeed * (transform.localScale.x / transform.localScale.magnitude));
            }
            
        }
        else
        {
            an.SetBool("Moving", false);
        }
        if (Input.GetKey(KeyCode.Space) && !an.GetBool("Moving"))
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
        else if (Input.GetKeyDown(KeyCode.E))
        {
            float bowAnimDurWait = 0.555333f;
            StartCoroutine(arrowSpawn(bowAnimDurWait));
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
    internal IEnumerator arrowSpawn(float dur)
    {
        an.SetTrigger("BowAttack");
        yield return new WaitForSeconds(dur);
        GameObject arrow = Instantiate(Arrow, transform.position, Quaternion.Euler(new Vector3(90, 0, 0)), null);
        Physics.IgnoreCollision(transform.GetComponent<BoxCollider>(), arrow.GetComponent<BoxCollider>(), true);
        arrow.GetComponent<Rigidbody>().velocity = transform.right * projSpeed * (transform.localScale.x / transform.localScale.magnitude);
        Destroy(arrow, 10);
    }
    internal IEnumerator rollSpeed(float dur)
    {
        yield return new WaitForSeconds(dur);
        transform.GetComponent<Rigidbody>().velocity = new Vector3(Mathf.Clamp(transform.GetComponent<Rigidbody>().velocity.x, -playerSpeed, playerSpeed), 0, Mathf.Clamp(transform.GetComponent<Rigidbody>().velocity.z, -playerSpeed, playerSpeed));
    }
}
