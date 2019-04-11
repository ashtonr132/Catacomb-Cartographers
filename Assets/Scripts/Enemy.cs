using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        AxeBandit, EarthWisp, EyeDemon, FireWisp, Goblin, Kobold, Mimic, Minataur, Oculothorax, Ogre, Slime, Sorcerer, WaterWisp, WindWisp 
    }
    public EnemyType type;
    public int maxHealth, currentHealth, projSpeed = 15;
    public float damage, moveSpeed;

    internal void AttackColl()
    {
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
