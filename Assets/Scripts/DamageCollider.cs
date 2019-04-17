using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.name.Contains("DamageCollider"))
        {
            if (other.transform.parent.name.Contains("Player") && transform.parent.tag.Contains("Enemy"))
            {
                GetComponentInParent<Enemy>().takeDamage(other.GetComponentInParent<PlayerControls>().damage);
            }
            else if (other.transform.parent.name.Contains("Enemy") && transform.parent.tag.Contains("Player"))
            {
                GetComponentInParent<PlayerControls>().takeDamage(other.GetComponentInParent<Enemy>().damage);
            }
        }
    }
}
