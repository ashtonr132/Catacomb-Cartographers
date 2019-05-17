using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    internal static PlayerControls pcs;
    private Enemy ens;
    private float Rdctn = 0.66f;

    private void Start()
    {
        if (transform.parent.parent.gameObject.name.Contains("Player")) //instance on the player sets the static player controls script access for attacking or recieving damage        
        {
            pcs = transform.GetComponentInParent<PlayerControls>();
        }
        else
        {
            ens = GetComponentInParent<Enemy>(); //sets non static reference to this enemy for attacking or recieving damage
            pcs = Enemy.Player.GetComponentInChildren<PlayerControls>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (transform.tag.Contains("Enemy") && other.transform.tag.Contains("Player")) //player hit this enemy
        {
            if (other.name.Contains("Proj")) //player proj hits this enemy
            {
                doDamage(pcs.projDamage * 15f, ens.pDmgRes, pcs.trueDamagePC, pcs.criticalStrikePC, pcs.criticalMultiplier * Rdctn);
                Destroy(other.transform.parent.gameObject);
            }
            else if (other.name.Contains("Melee")) //player melee hits this enemy
            {
                doDamage(pcs.meleeDamage * 15f, ens.mDmgRes, pcs.trueDamagePC * Rdctn, pcs.criticalStrikePC, pcs.criticalMultiplier);
            }
        }
        else if(transform.tag.Contains("Player") && other.transform.tag.Contains("Enemy")) //enemy hit this player
        {
            ens = other.GetComponentInParent<Enemy>();
            if (other.name.Contains("Proj"))
            {
                doDamage(ens.projDamage * 3, pcs.pDmgRes, ens.trueDmgPC, ens.criticalStrikePC, ens.criticalMultiplier * Rdctn, true);
                Destroy(other.transform.parent.gameObject);
            }
            else if (other.name.Contains("Melee"))
            {
                doDamage(ens.meleeDamage * 3, pcs.mDmgRes, ens.trueDmgPC * Rdctn, ens.criticalStrikePC, ens.criticalMultiplier, true);
            }
        }
    }

    private void doDamage(float damage, float res, float truePercent, float criticalStrikeChance, float criticalMultiplier, bool playertake = false)
    {
        damage += Random.Range(0f, 3f);
        float critical = Random.Range(0, 100) < criticalStrikeChance ? criticalMultiplier : 1f;
        float dmg = damage/100*(100-truePercent)/100*(100-res) + (damage * critical) + (damage/100*truePercent);
        if (playertake)
        {
            pcs.takeDamage(dmg);
        }
        else
        {
            ens.takeDamage(dmg);
        }
        int d = (int)dmg;
    }
}
