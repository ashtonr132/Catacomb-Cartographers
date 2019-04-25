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

    internal float currentHealth, lastHealth, projSpeed = 15, projDuration = 10, meleeDamage = 1, projDamage = 0.5f, mDmgRes = 5, pDmgRes = 5, trueDamagePC = 5, criticalStrikePC = 5, criticalMultiplier = 1.2f;
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
        if (transform.parent.rotation != Quaternion.identity) //cleanups
        {
            transform.parent.rotation = Quaternion.identity;
        }
        if (transform.position != transform.parent.position)
        {
            transform.position = transform.parent.position;
        }
        if (currentHealth < lastHealth)
        {
            updateUI(hp: currentHealth);
        }

        lastHealth = currentHealth;
        if (an.GetBool("Moving"))
        {
            
            GetComponent<SpriteRenderer>().flipX = transform.parent.GetComponent<NavMeshAgent>().destination.x >= transform.position.x ? false : true; //side facing
        }
        else
        { 
            GetComponent<SpriteRenderer>().flipX = Input.mousePosition.x > Screen.width / 2 ? false : true; //side facing
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            an.SetBool("Moving", false);
            if (an.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle2")
            {
                an.SetTrigger("Attack" + Random.Range(1, 4).ToString());
                StartCoroutine(MeleeAttack());
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
            rb.velocity = transform.position - transform.parent.GetComponent<NavMeshAgent>().destination * (playerSpeed + Random.Range(7, 11));
        }
        else if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
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
                Vector3 directionV = Vector3.zero;
                if (Input.GetKey(KeyCode.W))
                {
                    directionV.z++;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    directionV.z--;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    directionV.x++;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    directionV.x--;
                }
                if (Physics.Raycast(transform.parent.position + directionV, Vector3.down, out hit) && hit.transform.name.Contains("Level"))
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
        Destroy(arrow, projDuration);
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
    internal void takeDamage(float dmg)
    {
        GetComponent<Animator>().SetTrigger("Hurt");
        currentHealth -= dmg;
        if (currentHealth <= 0)
        {
            GetComponent<Animator>().SetTrigger("Die");
        }
    }
    private IEnumerator MeleeAttack()
    {
        transform.GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        transform.GetChild(1).gameObject.SetActive(false);
    }
}
