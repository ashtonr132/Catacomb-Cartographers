using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStuff : MonoBehaviour
{
    [SerializeField]
    internal GameObject player;
    internal float cameraDist = 3.5f;
    internal Quaternion CameraAngle = Quaternion.identity;

    // Update is called once per frame
    void Update()
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
                position.x = Mathf.Lerp(transform.position.x, player.transform.position.x, Time.deltaTime * Vector3.Distance(transform.position, player.transform.position) / 2);
                position.y = player.transform.position.y + cameraDist * 2;
                position.z = Mathf.Lerp(transform.position.z, player.transform.position.z -1, Time.deltaTime * Vector3.Distance(transform.position, player.transform.position) / 2);
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
