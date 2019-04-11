using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour
{
    private Rigidbody rb;
    internal float playerSpeed = 3;
    private Animator an;
    internal int currentHealth, lastHealth, projSpeed = 15;
    internal static int maxHealth = 100;
    [SerializeField]
    GameObject Arrow;
    GameObject canvas;
	// Use this for initialization
	void Start ()
    {
        currentHealth = maxHealth;
        lastHealth = currentHealth;
        rb = transform.GetComponent<Rigidbody>();
        an = transform.GetComponent<Animator>();
        canvas = GameObject.Find("Canvas");
        canvas.transform.GetChild(1).gameObject.SetActive(true);
	}

    // Update is called once per frame
    void Update()
    {
        transform.parent.rotation = Quaternion.identity;
        if (currentHealth < lastHealth)
        {
            updateUI(hp:currentHealth);
        }
        lastHealth = currentHealth;
        if (Input.mousePosition.x > Screen.width/2)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y,transform.localScale.z);
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            an.SetBool("Moving", false);
            if (an.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle2")
            {
                an.SetTrigger("Attack" + Random.Range(1, 4).ToString());
            }
            else if (an.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle1")
            {
                an.SetTrigger("Draw");
            }
        }
        else if (Input.GetKey(KeyCode.Mouse1))
        {
            an.SetBool("Moving", false);
            if (an.GetBool("AttackMode"))
            {
                an.SetTrigger("Sheathe");
            }
            an.SetBool("BowAttack", true);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            an.SetBool("Moving", false);
            an.SetTrigger("Cast");
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            an.SetBool("CastLoop", true);
        }
        else if(Input.GetKeyDown(KeyCode.C))
        {
            an.SetBool("Moving", false);
            an.SetBool("Use", true);
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
            rb.velocity = (velo / velo.magnitude) * (playerSpeed + Random.Range(4, 7));
        }
        else if (Vector3.Distance(Input.mousePosition, new Vector3(Screen.width / 2, Screen.height / 2, 0)) > (Screen.width+Screen.height)/15)
        {
            if (an.GetBool("AttackMode") && !an.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Sheathe"))
            {
                an.SetTrigger("Sheathe");
                an.SetBool("Moving", false);
            }
            else if (!an.GetBool("AttackMode") && (an.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Idle1") || an.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Move")) && !an.GetBool("Use"))
            {
                an.SetBool("Moving", true);
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit) && hit.transform.name.Contains("Level"))
                {
                    transform.parent.GetComponent<NavMeshAgent>().SetDestination(hit.point);
                }
            }
        }
        else
        {
            an.SetBool("Moving", false);
        }
        if (!an.GetBool("Moving"))
        {
            transform.parent.GetComponent<NavMeshAgent>().velocity = Vector3.zero;
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
        arrow.GetComponent<Rigidbody>().velocity = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized * projSpeed;
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
    internal void Sheathe(int b)
    {
        bool c = b == 1 ? true : false;
        an.SetBool("AttackMode", c);
    }
    internal void bowAttack()
    {
        an.SetBool("BowAttack", false);
    }
    internal void use()
    {
        an.SetBool("Use", false);
    }
    internal void updateUI(float? hp = null, float? rp = null)
    {
        if (hp != null)
        {
            canvas.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Slider>().value = hp.Value;
        }
        if (rp != null)
        {
            canvas.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Slider>().value = rp.Value;
        }
    }
}
