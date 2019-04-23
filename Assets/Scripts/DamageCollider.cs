using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
            if (transform.tag.Contains("Enemy") && other.transform.tag.Contains("Player")) //player hit this enemy
            {
                if (other.name.Contains("Proj")) //player proj hits this enemy
                {
                    GetComponentInParent<Enemy>().takeDamage(
                        (Enemy.Player.GetComponentInChildren<PlayerControls>().projDamage / 100)
                        * (100 - GetComponentInParent<Enemy>().pDmgRes));
                }
                else if (other.name.Contains("Melee")) //player melee hits this enemy
                {
                    GetComponentInParent<Enemy>().takeDamage(
                        (other.GetComponentInParent<PlayerControls>().meleeDamage / 100)
                        * (100 - GetComponent<Enemy>().mDmgRes));
                }
            }
            else if(transform.tag.Contains("Player") && other.transform.tag.Contains("Enemy")) //enemy hit this player
            {
                if (other.name.Contains("Proj"))
                {
                    GetComponentInParent<PlayerControls>().takeDamage(
                        (other.GetComponentInParent<Enemy>().projDamage / 100) 
                        * (100 - GetComponentInParent<PlayerControls>().pDmgRes));
                }
                else if (other.name.Contains("Melee"))
                {
                    GetComponentInParent<PlayerControls>().takeDamage(
                        (other.GetComponentInParent<Enemy>().meleeDamage / 100) 
                        * (100 - GetComponentInParent<PlayerControls>().mDmgRes));
                }
            }
        
    }
}
