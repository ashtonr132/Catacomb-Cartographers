using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        AxeBandit, EarthWisp, EyeDemon, FireWisp, Goblin, Kobold, Minataur, Oculothorax, Ogre, Slime, Sorcerer, WaterWisp, WindWisp, Mimic, unassigned 
    }

    public EnemyType type = EnemyType.unassigned;
    internal float maxHealth = 100, currentHealth, projSpeed = 10, projDuration = 10, visionRange = 5, projDamage = 0.5f, meleeDamage = 1, aggressiveness = 1, mDmgRes = 1, pDmgRes = 1, moveSpeed = 1, trueDmgPC = 0, criticalStrikePC = 5;
    private GameObject Projectile = null;
    private bool isRanged = false;
    internal static GameObject Player;

    private void Start()
    {
        currentHealth = maxHealth;
        if (Player == null)
        {
            Player = GameObject.Find("Player");
        }
        if (name.Contains("Wisp"))
        {
            isRanged = true;
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
    }

    private IEnumerator RangedAttack()
    {
        if (Projectile == null)
        {
            foreach (GameObject proj in GetComponentInParent<Spawner>().Projectiles)
            {
                if (proj.name.Contains(transform.name))
                {
                    Projectile = proj;
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
        GameObject nproj = Instantiate(Projectile, transform.position, Quaternion.Euler(new Vector3(90, 0, 0)), null);
        Physics.IgnoreCollision(transform.GetComponent<BoxCollider>(), nproj.GetComponent<BoxCollider>(), true);
        nproj.GetComponent<Rigidbody>().velocity = (transform.position - Player.transform.position).normalized * projSpeed;
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
        }
    }
}
