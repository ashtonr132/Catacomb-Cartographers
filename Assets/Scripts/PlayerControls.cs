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

    internal float currentHealth, lastHealth, projSpeed = 2, projDuration = 10, meleeDamage = 1, projDamage = 0.5f, mDmgRes = 5, pDmgRes = 5, trueDamagePC = 5, criticalStrikePC = 5, criticalMultiplier = 1.2f, maxrp = 1000, rp;
    internal static int maxHealth = 1000, experience, spawners;
    internal static float recentHits = 0, maxShroud, shroud;
    internal static bool updateuiinfo = false;
    internal static Vector3 direction = Vector3.zero;
    [SerializeField]
    GameObject Arrow;
    GameObject canvas;

	// Use this for initialization
	void Start ()
    {
        currentHealth = maxHealth;
        lastHealth = currentHealth;
        rp = maxrp;
        rb = transform.GetComponent<Rigidbody>();
        an = transform.GetComponent<Animator>();
        canvas = GameObject.Find("Canvas");
        canvas.transform.GetChild(1).gameObject.SetActive(true);
        canvas.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = "Alpha Area";
        canvas.transform.GetChild(1).GetChild(1).GetChild(1).GetComponent<Text>().text = "Difficulty : " + GameFiles.saveData.Difficulty.ToString();
        updateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (updateuiinfo)
        {
            updateUI(currentHealth, rp);
            updateuiinfo = false;
        }
        if (recentHits > 0)
        {
            recentHits -= Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canvas.GetComponentInChildren<Menu>().inGameSettings();
            canvas.transform.GetChild(1).gameObject.SetActive(false);
        }
        if (transform.parent.rotation != Quaternion.identity) //cleanups
        {
            transform.parent.rotation = Quaternion.identity;
        }
        if (transform.position != transform.parent.position)
        {
            transform.position = transform.parent.position;
        }
        updateUI(hp: currentHealth);
        if (an.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Idle"))
        {
            rp += 10;
            updateUI(rp: rp);
        }
        if (an.GetBool("Moving"))
        {
            GetComponent<SpriteRenderer>().flipX = transform.parent.GetComponent<NavMeshAgent>().destination.x >= transform.position.x ? false : true; //side facing
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = Input.mousePosition.x > Screen.width / 2 ? false : true; //side facing
        }
        if (Input.GetKey(KeyCode.Mouse0) && rp >= 50)
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
        else if (Input.GetKey(KeyCode.Mouse1) && rp >= 50)
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
        else if (Input.GetKeyDown(KeyCode.C) && rp >= 400)
        {
            rp -= 400;
            updateUI(rp: rp);
            an.SetBool("Moving", false);
            an.SetBool("Use", true);
            if (currentHealth + maxHealth / 4 < maxHealth)
            {
                currentHealth += maxHealth / 4;
            }
            else
            {
                currentHealth = maxHealth;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !an.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Slide") && rp >= 75)
        {
            rp -= 75;
            updateUI(rp: rp);
            if (Random.value > 0.5f)
            {
                an.SetTrigger("Slide");
            }
            else
            {
                an.SetTrigger("Slide2");
            }
            rb.velocity = transform.position - transform.parent.GetComponent<NavMeshAgent>().destination * (playerSpeed + Random.Range(70, 110));
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
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
                    directionV.z += 0.2f;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    directionV.z -= 0.2f;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    directionV.x += 0.2f;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    directionV.x -= 0.2f;
                }
                direction = directionV;
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
        else
        {
            rp -= 0.25f;
            updateUI(rp: rp);
        }
    }
    internal void arrowSpawn()
    {
        rp -= 50;
        updateUI(rp: rp);
        GameObject arrow = Instantiate(Arrow, transform.position, Quaternion.identity, null);
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            arrow.transform.LookAt(hit.point, transform.forward);
        }
        arrow.GetComponent<Rigidbody>().velocity = arrow.transform.forward * projSpeed;
        arrow.transform.Rotate(90, -90, 0);
        Destroy(arrow, projDuration);
    }
    internal void clampSpeed()
    {
        rb.velocity = new Vector3(Mathf.Clamp(transform.GetComponent<Rigidbody>().velocity.x, -playerSpeed, playerSpeed), 0, Mathf.Clamp(transform.GetComponent<Rigidbody>().velocity.z, -playerSpeed, playerSpeed));
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
        lastHealth = currentHealth;
        if (hp != null)
        {
            if (hp < maxHealth)
            {
                hp += Time.deltaTime*10;
            }
            canvas.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Slider>().value = hp.Value/maxHealth;
        }
        if (rp != null)
        {
            canvas.transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Slider>().value = rp.Value/maxrp;
        }
        canvas.transform.GetChild(1).GetChild(1).GetChild(2).GetComponent<Text>().text = "Mapped : " + System.Math.Round((System.Decimal)(shroud / maxShroud)*100, 2, System.MidpointRounding.AwayFromZero) + "%";
        canvas.transform.GetChild(1).GetChild(1).GetChild(3).GetComponent<Text>().text = "Spawners : " + spawners.ToString();

        canvas.transform.GetChild(1).GetChild(0).GetChild(2).GetChild(0).GetComponent<Text>().text = "EXP : " + GameFiles.saveData.Experience.ToString();
    }
    internal void takeDamage(float dmg)
    {
        if (!an.GetCurrentAnimatorClipInfo(0)[0].clip.name.Contains("Hurt"))
        {
            an.SetTrigger("Hurt");
            currentHealth -= dmg;
            if (currentHealth <= 0)
            {
                an.SetTrigger("Die");
            }
            else
            {
                DamageNums.CreateDamageText(((int)dmg).ToString(), transform.position);
            }
        }
    }
    private IEnumerator MeleeAttack()
    {
        rp -= 50;
        updateUI(rp: rp);
        transform.GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        transform.GetChild(1).gameObject.SetActive(false);
    }
}
