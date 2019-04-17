﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType // 0 ranged, 1 melee, 2 cannot attack
    {
        AxeBandit, EarthWisp, EyeDemon, FireWisp, Goblin, Kobold, Minataur, Oculothorax, Ogre, Slime, Sorcerer, WaterWisp, WindWisp, Mimic, unassigned 
    }

    public EnemyType type = EnemyType.unassigned;
    internal float maxHealth = 100, currentHealth, projSpeed = 15, visionRange = 5, damage = 1, aggressiveness = 1;
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
        else if(transform.GetChild(0).name.Contains("Melee"))
        {
            //transform.GetChild(0).gameObject.AddComponent<AttackCollider>();
        }
    }

    internal void AttackColl()
    {
        if (type != EnemyType.unassigned)
        {
            if (!isRanged)
            {
                StartCoroutine(MeleeAttack());
            }
            else
            {
                StartCoroutine(RangedAttack());
            }
        }
    }

    private IEnumerator MeleeAttack()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        transform.GetChild(0).gameObject.SetActive(false);
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
        Destroy(nproj, 10);
    }

    internal void takeDamage(float dmg)
    {
        GetComponent<Animator>().SetTrigger("Hurt");
        currentHealth -= dmg;
        if (currentHealth <= 0)
        {
            try
            {
                GetComponent<Animator>().SetTrigger("Die");
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}