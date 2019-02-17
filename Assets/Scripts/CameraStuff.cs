using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStuff : MonoBehaviour
{
    [SerializeField]
    internal GameObject player;
    internal float cameraDist = 3.5f;

    // Update is called once per frame
    void Update()
    {
        if (Menu.GameStarted)
        {
            if (player == null)
            {
                player = GameObject.Find("Player");
            }
            else
            {
                try
                {
                    Vector3 position = transform.position;
                    int levelSize = LevelGen.levelSize / 2, cap = 5;
                    position.x = Mathf.Clamp(Mathf.Lerp(transform.position.x, player.transform.position.x, Time.deltaTime * Vector3.Distance(transform.position, player.transform.position) / 2), -levelSize + cap, levelSize - cap);
                    position.y = player.transform.position.y + cameraDist * 2;
                    position.z = Mathf.Clamp(Mathf.Lerp(transform.position.z, player.transform.position.z - 1, Time.deltaTime * Vector3.Distance(transform.position, player.transform.position) / 2), -levelSize + cap, levelSize - cap);
                    transform.position = position;
                    transform.rotation = Quaternion.LookRotation(player.transform.forward, Vector3.up);
                }
                catch
                {
                    throw new ArgumentException("Player has not been instantiated in scene.", "Camera Stuff Script");
                }
            }
        }
    }
}
