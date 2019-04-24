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
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (transform.tag.Contains("Enemy") && other.transform.tag.Contains("Player")) //player hit this enemy
        {
            if (other.name.Contains("Proj")) //player proj hits this enemy
            {
                doDamage(pcs.projDamage, ens.pDmgRes, pcs.trueDamagePC, pcs.criticalStrikePC * Rdctn);
            }
            else if (other.name.Contains("Melee")) //player melee hits this enemy
            {
                doDamage(pcs.meleeDamage, ens.mDmgRes, pcs.trueDamagePC * Rdctn, pcs.criticalStrikePC);
            }
        }
        else if(transform.tag.Contains("Player") && other.transform.tag.Contains("Enemy")) //enemy hit this player
        {
            if (other.name.Contains("Proj"))
            {
                doDamage(ens.projDamage, pcs.pDmgRes, ens.trueDmgPC, ens.criticalStrikePC * Rdctn, true);
            }
            else if (other.name.Contains("Melee"))
            {
                doDamage(ens.meleeDamage, pcs.mDmgRes, ens.trueDmgPC * Rdctn, ens.criticalStrikePC, true);
            }
        }
    }
    private void doDamage(float damage, float res, float truePercent, float criticalStrikeChance, bool playertake = false)
    {
        var dmg = (damage/100*(100-truePercent)/100*(100-res)) + (damage/100*truePercent) * criticalStrikeChance;
        if (playertake)
        {
            pcs.takeDamage(dmg);
        }
        else
        {
            ens.takeDamage(dmg);
        }
        DamageNums.CreateDamageText(dmg.ToString(), transform.position);
    }
}
