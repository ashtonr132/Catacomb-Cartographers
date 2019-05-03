using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        AxeBandit, EarthWisp, FireWisp, Goblin, Kobold, Minataur, Oculothorax, Ogre, Slime, Sorcerer, WaterWisp, WindWisp, EyeDemon, Mimic, unassigned 
    }

    public enum EnemyState
    {
        Attack, Chase, Flee, Return, Idle, Wander
    }

    public EnemyState state = EnemyState.Wander;
    public EnemyType type = EnemyType.unassigned;
    internal float maxHealth = 100, currentHealth, projSpeed = 10, projDuration = 10, visionRange = 1, projDamage = 5, meleeDamage = 10, aggressiveness = 50, mDmgRes = 1, pDmgRes = 1, moveSpeed = 1, trueDmgPC = 0, criticalStrikePC = 5, criticalMultiplier = 1.2f;
    internal GameObject Projectile = null, UIOverlay;
    private bool isRanged = false, actionComplete = false;
    internal static GameObject Player;
    internal float meleeAttackRange = float.MinValue, leashDist = LevelGen.levelSize / 12, lastHealth;
    private Animator an;

    private void Start()
    {
        foreach (Transform c in transform)
        {
            if (c.name.Contains("Melee"))
            {
                isRanged = false;
                meleeAttackRange = 1;
                break;
            }
            isRanged = true;
        }
        currentHealth = maxHealth;
        lastHealth = maxHealth;
        if (Player == null)
        {
            Player = GameObject.Find("Player");
        }
        an = GetComponent<Animator>();
    }

    private void Update()
    {
        gameObject.GetComponent<SpriteRenderer>().flipX = Player.transform.position.x > transform.position.x ? true : false;
        if (UIOverlay != null)
        {
            UIOverlay.transform.position = //-CameraFollow.offset + 
                Camera.main.WorldToScreenPoint(transform.parent.position + transform.parent.up);
            UIOverlay.GetComponent<Text>().text = "State = " + state.ToString() + "\nHealth = " + currentHealth;
        }
        if (transform.parent.rotation != Quaternion.identity) //cleanups
        {
            transform.parent.rotation = Quaternion.identity;
        }
        if (transform.position != transform.parent.position)
        {
            transform.position = transform.parent.position;
        }
        if (name.Contains("Sorcerer"))
        {
            an.SetBool("Move", true);
        }
        if (actionComplete)
        {
            state = determineState();
            switch (state)
            {
                case EnemyState.Attack:
                    actionComplete = false;
                    an.SetBool("Move", false);
                    an.SetTrigger("Attack");
                    break;

                case EnemyState.Chase:
                    actionComplete = false;
                    an.SetBool("Move", true);
                    transform.parent.GetComponent<NavMeshAgent>().SetDestination(Player.transform.position);
                    break;

                case EnemyState.Flee:
                    actionComplete = false;

                    an.SetBool("Move", true);
                    transform.parent.GetComponent<NavMeshAgent>().SetDestination((transform.position - Player.transform.position).normalized * Random.Range(1.2f, 1.5f));
                    break;

                case EnemyState.Return:
                    actionComplete = false;

                    an.SetBool("Move", true);

                    transform.parent.GetComponent<NavMeshAgent>().SetDestination(transform.parent.parent.position);
                    break;

                case EnemyState.Wander:
                    actionComplete = false;

                    an.SetBool("Move", true);

                    newDest(0);
                    actionComplete = false;
                    break;

                default:
                    an.SetBool("Move", false);
                    actionComplete = false;
                    StartCoroutine(Idle(Random.Range(3, 5)));
                    break;
            }
        }
        else
        {
            if (transform.parent.GetComponent<NavMeshAgent>().path.status == NavMeshPathStatus.PathComplete && state != EnemyState.Attack && state != EnemyState.Idle)
            {
                actionComplete = true;
                    an.SetBool("Move", false);
            }
        }
    }

    private void newDest(float i)
    {
        i++;
        if (i < 35)
        {
            RaycastHit hit;
            Vector2 ranUIC = Random.insideUnitCircle;
            Vector3 testDest = transform.position + new Vector3(ranUIC.x, 0, ranUIC.y);
            NavMeshPath p = new NavMeshPath();
            if (Physics.Raycast(testDest + Vector3.up, testDest + Vector3.down, out hit) && hit.transform.name.Contains("Level") && transform.parent.GetComponent<NavMeshAgent>().CalculatePath(testDest, p) && p.status == NavMeshPathStatus.PathComplete)
            {
                transform.parent.GetComponent<NavMeshAgent>().SetPath(p);

                if (transform.parent.GetComponent<NavMeshAgent>().remainingDistance > leashDist)
                {
                    newDest(i);
                }
            }
        }
        else
        {
            Debug.Log("severe navigation error no paths found 35 attemps");
        }
    }


    internal EnemyState determineState()
    {
        bool canSeePlayer = true, Aggressive = false;
        float dist = Vector2.Distance(Player.transform.position, transform.position);
        if (dist <= visionRange)
        {
            RaycastHit[] inSight = Physics.RaycastAll(new Ray(transform.position, Player.transform.position), visionRange);
            if (inSight != null && inSight.Length > 0)
            {
                for (int i = 0; i < inSight.Length; i++)
                {
                    if (inSight[i].transform.tag.Contains("ImpassableTerrain"))
                    {
                        canSeePlayer = false;
                    }
                }
            }
        }
        else
        {
            canSeePlayer = false;
        }
        if ((currentHealth+aggressiveness)/maxHealth <= (maxHealth/4))
        {
            Aggressive = true;
        }

        if (canSeePlayer)
        {
            if (!Aggressive && Random.value >= 0.25f)
            {
                return EnemyState.Flee;
            }
            else
            {
                if (isRanged || (!isRanged && dist < meleeAttackRange))
                {
                    return EnemyState.Attack;
                }
                else
                {
                    return EnemyState.Chase;
                }
            }
        }
        else
        {
            dist = Vector2.Distance(transform.position, transform.parent.position);
            if (dist > leashDist && Random.value < 0.66f)
            {
                return EnemyState.Return;
            }
            else if (dist < 1 && Random.value < 0.66f)
            {
                return EnemyState.Wander;
            }
            else
            {
                return EnemyState.Idle;
            }
        }
    }

    internal void AttackColl()
    {
        if (transform.parent.rotation != Quaternion.identity) //cleanups
        {
            transform.parent.rotation = Quaternion.identity;
        }
        if (transform.position != transform.parent.position)
        {
            transform.position = transform.parent.position;
        }


        if (type != EnemyType.unassigned)
        {
            if (isRanged)
            {
                StartCoroutine(RangedAttack());
            }
            else
            {
                StartCoroutine(MeleeAttack());
            }
        }
    }

    private IEnumerator MeleeAttack()
    {
        transform.GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        transform.GetChild(1).gameObject.SetActive(false);
        actionComplete = true;

    }

    private IEnumerator RangedAttack()
    {
        if (Projectile == null)
        {
            foreach (GameObject proj in GetComponentInParent<Spawner>().Projectiles)
            {
                var s = proj.name.Substring(proj.name.ToCharArray().Length - 4);
                if (transform.name.Contains(s));
                {
                    Projectile = proj;
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
        actionComplete = true;
        GameObject nproj = Instantiate(Projectile, transform.position, Quaternion.Euler(new Vector3(90, 0, 0)), null);
        nproj.GetComponent<Rigidbody>().velocity = (Player.transform.position - transform.position).normalized * projSpeed;
        Destroy(nproj, projDuration);
    }

    internal void takeDamage(float dmg)
    {
        try
        {
            GetComponent<Animator>().SetTrigger("Hurt");
        }
        catch (System.Exception)
        {
            throw; //animation missing ignore indev
        }
        currentHealth -= dmg;
        if (currentHealth <= 0)
        {
            try
            {
                GetComponent<Animator>().SetTrigger("Die");
            }
            catch (System.Exception)
            {

                throw; //animation missing ignore indev
            }
            GameFiles.saveData.Experience++;
            Destroy(UIOverlay.gameObject);
            Destroy(gameObject.transform.parent.gameObject, 3);
        }
    }
    internal IEnumerator Idle(float wait)
    {
        yield return new WaitForSeconds(wait);
        actionComplete = true;
    }
}
