using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStuff : MonoBehaviour
{
    [SerializeField]
    internal GameObject player;
    internal float cameraDist = 3.5f, camSpeed = 2;

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
                    int levelSize = LevelGen.levelSize / 2;
                    position.x = Mathf.Clamp(Mathf.SmoothStep(transform.position.x, player.transform.position.x, Time.deltaTime * Vector3.Distance(transform.position, player.transform.position) / 2), -levelSize, levelSize);
                    position.y = player.transform.position.y + cameraDist * 2;
                    position.z = Mathf.Clamp(Mathf.SmoothStep(transform.position.z, player.transform.position.z, Time.deltaTime * Vector3.Distance(transform.position, player.transform.position) / 2), -levelSize, levelSize);
                    transform.position = position;
                    Vector3 rotation = transform.rotation.eulerAngles;
                    Vector3 lr = player.transform.rotation.eulerAngles;
                    rotation.x = Mathf.Lerp(rotation.x, lr.x, Time.deltaTime * camSpeed);
                    rotation.y = Mathf.Lerp(rotation.y, lr.y, Time.deltaTime * camSpeed);
                    rotation.z = Mathf.Lerp(rotation.z, lr.z, Time.deltaTime * camSpeed);
                    transform.rotation = Quaternion.Euler(rotation);
                }
                catch
                {
                    throw new ArgumentException("Player has not been instantiated in scene.", "Camera Stuff Script");
                }
            }
        }
    }
}
