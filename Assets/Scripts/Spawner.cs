using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] Enemies;
    int spawnCap = 5, currentCount = 0;

    internal IEnumerator startSpawn(float wait, int spawn)
    {
        int enemy = Random.Range(0, Enemies.Length), i = 0;
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
                        yield return new WaitForSeconds(wait);
                        GameObject e = Instantiate(Enemies[enemy], transform.position + new Vector3(n.x, 0.5f, n.y), Quaternion.Euler(90, 0, 0), transform);
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
