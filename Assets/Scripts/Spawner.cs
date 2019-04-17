using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    internal GameObject[] Enemies, Projectiles;
    int spawnCap = 5, currentCount = 0;
    int? enemy = null;

    internal IEnumerator startSpawn(float wait, int spawn)
    {
        if (enemy == null)
        {
            enemy = Random.Range(0, Enemies.Length - 2);
            transform.name = "Spawner " + Enemies[enemy.Value].name;
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
                        GameObject e = Instantiate(Enemies[enemy.Value], transform.position + new Vector3(n.x, 0.5f, n.y), Quaternion.Euler(90, 0, 0), transform);
                        e.AddComponent<Enemy>().type = (Enemy.EnemyType)System.Enum.GetValues(typeof(Enemy.EnemyType)).GetValue(enemy.Value);
                        e.name = Enemies[enemy.Value].name;
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
}
