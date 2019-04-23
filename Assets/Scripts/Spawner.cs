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
            type = Random.Range(0, Enemies.Length - 2);
            transform.name = "Spawner " + Enemies[type.Value].name;
        }
        int i = 0;
        for (;;)
        {
            var n = Random.insideUnitCircle * 3;
            RaycastHit hit;
            if (currentCount < spawnCap && i < spawn)
            {
                if (Physics.Raycast(transform.position + new Vector3(n.x, 1, n.y), Vector3.down, out hit, Mathf.Infinity))
                {
                    if (hit.transform.name.Contains("Level"))
                    {
                        yield return new WaitForSeconds(wait + Random.Range(0, wait/8));
                        GameObject e = Instantiate(Enemies[type.Value], transform.position + new Vector3(n.x, 0.5f, n.y), Quaternion.Euler(90, 0, 0), transform);
                        e.AddComponent<Enemy>().type = (Enemy.EnemyType)System.Enum.GetValues(typeof(Enemy.EnemyType)).GetValue(type.Value);
                        StartCoroutine(setStats(e.GetComponent<Enemy>(), 0));
                        i++;
                        currentCount++;
                    }
                }
            }
            else
            {
                break;
            }
        }
    }

    internal IEnumerator setStats(Enemy e, int recs)
    {
        e.name = e.type + " " + currentCount.ToString();
        int diff = GameFiles.saveData.Difficulty;
        switch (e.type)
        {
            case Enemy.EnemyType.AxeBandit:

                e.maxHealth = 80 * diff;
                e.visionRange = 5;
                e.meleeDamage = 0.8f * diff;
                e.pDmgRes = 18;
                e.mDmgRes = 12;
                e.moveSpeed = 5;
                break;

            case Enemy.EnemyType.EarthWisp:
                e.maxHealth = 70 * diff;
                e.projSpeed = 8 * diff;
                e.projDamage = 0.4f * diff;
                e.projDuration = 10;
                e.visionRange = 7;
                e.pDmgRes = 10;
                e.mDmgRes = 12;
                e.moveSpeed = 4;
                break;

            case Enemy.EnemyType.EyeDemon:

                e.maxHealth = 40 * diff;
                e.visionRange = 0;
                e.pDmgRes = 0;
                e.mDmgRes = 0;
                e.moveSpeed = 0;
                break;

            case Enemy.EnemyType.FireWisp:

                e.maxHealth = 60 * diff;
                e.projSpeed = 10 * diff;
                e.projDamage = 0.75f * diff;
                e.projDuration = 7;
                e.visionRange = 8;
                e.mDmgRes = -15;
                e.moveSpeed = 6;
                break;

            case Enemy.EnemyType.Goblin:
                e.maxHealth = 80 * diff;
                e.visionRange = 4;
                e.meleeDamage = 0.25f * diff;
                e.pDmgRes = 15;
                e.mDmgRes = 15;
                e.moveSpeed = 8;
                break;

            case Enemy.EnemyType.Kobold:
                e.maxHealth = 80 * diff;
                e.visionRange = 5;
                e.meleeDamage = 1;
                e.pDmgRes = 17;
                e.mDmgRes = 13;
                e.moveSpeed = 7;
                break;

            case Enemy.EnemyType.Minataur:
                e.maxHealth = 120 * diff;
                e.visionRange = 5;
                e.meleeDamage = 1.25f * diff;
                e.pDmgRes = 65;
                e.mDmgRes = 55;
                e.moveSpeed = 3;
                break;

            case Enemy.EnemyType.Oculothorax:
                e.maxHealth = 30 * diff;
                e.visionRange = 5;
                e.meleeDamage = 1.15f * diff;
                e.pDmgRes = 15;
                e.mDmgRes = -10;
                e.moveSpeed = 12;
                break;

            case Enemy.EnemyType.Ogre:
                e.maxHealth = 150 * diff;
                e.visionRange = 3;
                e.meleeDamage = 0.5f;
                e.pDmgRes = -30;
                e.mDmgRes = 40;
                e.moveSpeed = 2;
                break;

            case Enemy.EnemyType.Slime:
                e.maxHealth = 50 * diff;
                e.visionRange = 5;
                e.meleeDamage = 1;
                e.pDmgRes = 25;
                e.mDmgRes = 25;
                e.moveSpeed = 4;
                break;

            case Enemy.EnemyType.Sorcerer:
                e.maxHealth = 100 * diff;
                e.visionRange = 12;
                e.moveSpeed = 5;
                break;

            case Enemy.EnemyType.WaterWisp:
                e.maxHealth = 65 * diff;
                e.projSpeed = 10;
                e.projDamage = 0.5f;
                e.projDuration = 10;
                e.visionRange = 5;
                e.pDmgRes = 32;
                e.mDmgRes = 32;
                e.moveSpeed = 4;
                break;

            case Enemy.EnemyType.WindWisp:
                e.maxHealth = 65 * diff;
                e.projSpeed = 10;
                e.projDamage = 0.5f;
                e.projDuration = 10;
                e.visionRange = 5;
                e.pDmgRes = -25;
                e.mDmgRes = 60;
                e.moveSpeed = 4;
                break;

            case Enemy.EnemyType.Mimic:
                e.maxHealth = 100 * diff;
                e.visionRange = 100;
                e.meleeDamage = 0.9f * diff;
                e.pDmgRes = 99.99f;
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
