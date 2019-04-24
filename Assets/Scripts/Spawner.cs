using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    internal GameObject[] Enemies, Projectiles;
    int spawnCap = 5, currentCount = 0;
    int? type = null;

    internal IEnumerator startSpawn(float wait, int spawn)
    {
        if (type == null)
        {
            type = Random.Range(0, Enemies.Length - 1);
            transform.name = "Spawner " + Enemies[type.Value].name;
        }
        for (int i = 0; i < spawn;)
        {
            if (currentCount < spawnCap)
            {
                yield return new WaitForSeconds(wait + Random.Range(0, wait));
                GameObject e = Instantiate(Enemies[type.Value], transform.position, Quaternion.identity, transform);
                e = e.transform.GetChild(0).gameObject;
                e.transform.rotation = Quaternion.Euler(90, 0, 0);
                e.AddComponent<Enemy>().type = (Enemy.EnemyType)System.Enum.GetValues(typeof(Enemy.EnemyType)).GetValue(type.Value);
                StartCoroutine(setStats(e.GetComponent<Enemy>(), 0));
                i++;
                currentCount++;
            }
        }
                
    }

    internal IEnumerator setStats(Enemy e, int recs)
    {
        e.name = e.type + " " + currentCount.ToString();
        float diff = LevelGen.diff;
        switch (e.type)
        {
            case Enemy.EnemyType.AxeBandit:

                e.maxHealth = 80 * diff;
                e.meleeDamage = 0.8f * diff;
                e.pDmgRes = 1.8f * diff;
                e.mDmgRes = 1.2f * diff;

                e.moveSpeed = 5;
                break;

            case Enemy.EnemyType.EarthWisp:

                e.maxHealth = 70 * diff;
                e.projSpeed = 8 * diff;
                e.projDamage = 0.4f * diff;
                e.pDmgRes = 1.0f * diff;
                e.mDmgRes = 1.2f * diff;

                e.visionRange = 7;
                e.moveSpeed = 4;
                break;

            case Enemy.EnemyType.EyeDemon:

                e.maxHealth = 40 * diff;
                e.pDmgRes = 0;
                e.mDmgRes = 0;

                e.visionRange = 0;
                e.moveSpeed = 0;
                break;

            case Enemy.EnemyType.FireWisp:

                e.maxHealth = 60 * diff;
                e.projSpeed = 12 * diff;
                e.projDamage = 0.75f * diff;
                e.mDmgRes = -15f * diff;

                e.projDuration = 7;
                e.visionRange = 8;
                e.moveSpeed = 6;
                break;

            case Enemy.EnemyType.Goblin:

                e.maxHealth = 80 * diff;
                e.meleeDamage = 0.25f * diff;
                e.pDmgRes = 1.5f * diff;
                e.mDmgRes = 1.5f * diff;

                e.visionRange = 4;
                e.moveSpeed = 8;
                break;

            case Enemy.EnemyType.Kobold:

                e.maxHealth = 80 * diff;
                e.meleeDamage = 1 * diff;
                e.pDmgRes = 1.7f * diff;
                e.mDmgRes = 1.3f * diff;
                
                e.moveSpeed = 7;
                break;

            case Enemy.EnemyType.Minataur:

                e.maxHealth = 120 * diff;
                e.meleeDamage = 1.25f * diff;
                e.pDmgRes = 6.5f * diff;
                e.mDmgRes = 5.5f * diff;
                
                e.moveSpeed = 3;
                break;

            case Enemy.EnemyType.Oculothorax:

                e.maxHealth = 30 * diff;
                e.meleeDamage = 1.15f * diff;
                e.pDmgRes = 1.5f * diff;
                e.mDmgRes = -1.0f * diff;
                
                e.moveSpeed = 12;
                break;

            case Enemy.EnemyType.Ogre:

                e.maxHealth = 150 * diff;
                e.meleeDamage = 0.5f;
                e.pDmgRes = -3 * diff;
                e.mDmgRes = 8 * diff;

                e.visionRange = 3;
                e.moveSpeed = 2;
                break;

            case Enemy.EnemyType.Slime:

                e.maxHealth = 50 * diff;
                e.meleeDamage = 1 * diff;
                e.pDmgRes = 2.5f * diff;
                e.mDmgRes = 2.5f * diff;
                
                e.moveSpeed = 4;
                break;

            case Enemy.EnemyType.Sorcerer:

                e.maxHealth = 100 * diff;

                e.visionRange = 12;
                e.moveSpeed = 5;
                break;

            case Enemy.EnemyType.WaterWisp:

                e.maxHealth = 65 * diff;
                e.pDmgRes = 4 * diff;
                e.mDmgRes = 4 * diff;

                e.moveSpeed = 4;
                break;

            case Enemy.EnemyType.WindWisp:

                e.maxHealth = 65 * diff;
                e.pDmgRes = -2.5f * diff;
                e.mDmgRes = 6 * diff;

                e.moveSpeed = 4;
                break;

            case Enemy.EnemyType.Mimic:

                e.maxHealth = 100 * diff;
                e.meleeDamage = 0.9f * diff;

                e.pDmgRes = 99.99f;
                e.visionRange = 100;
                e.moveSpeed = 6;
                break;

            default:
                if (recs < 8) // two seconds potential unassigned enemytype, if enemy is still unassigned and calling recursivly respawn it.
                {
                    yield return new WaitForSeconds(0.25f);
                    StartCoroutine(setStats(e.GetComponent<Enemy>(), recs + 1));
                }
                else
                {
                    Destroy(e.gameObject);
                    StartCoroutine(startSpawn(1, 5));
                }
                Debug.Log("enemy spawned with no type persists");
                break;
        }
    }
}
