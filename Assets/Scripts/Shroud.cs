using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shroud : MonoBehaviour
{
    bool counts = false;
    private void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            if (hit.transform.name.Contains("Level") && !hit.transform.tag.Contains("ImpassableTerrain"))
            {
                counts = true;
                PlayerControls.maxShroud++;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (counts)
        {
            PlayerControls.shroud++;
        }
        PlayerControls.updateuiinfo = true;
        StartCoroutine(flip());
    }

    private IEnumerator flip()
    {
        var rot = transform.rotation;
        do
        {
            yield return new WaitForSeconds(0.1f);
            if (rot.z >= 180)
            {
                rot.x = 0;
                rot.z = 180;
            }
            else if (rot.x >= 180)
            {
                rot.z = 0;
                rot.x = 180;
            }
            else
            {
                if (Random.value > 0.5f)
                {
                    rot.x += Random.Range(1, 3);
                }
                else
                {
                    rot.z += Random.Range(1, 3);
                }
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
        } while (rot.x != 180 && rot.z != 180);
        transform.gameObject.SetActive(false);

    }
}
