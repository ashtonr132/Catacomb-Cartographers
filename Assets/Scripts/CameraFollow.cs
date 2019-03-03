using UnityEngine;
using System.Collections;
using System;

public class CameraFollow : MonoBehaviour
{
    private readonly float SmoothTime = 0.15f;
    private Vector3 CurrentVelocity = Vector3.zero;
    public Transform Target;

    // Update is called once per frame
    void Update()
    {
        if (Menu.GameStarted)
        {
            if (Target == null)
            {
                Target = GameObject.Find("Player").transform;
            }
            else
            {
                try
                {
                    var targetDestination = transform.position + Target.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Camera.main.WorldToViewportPoint(Target.position).z));
                    transform.position = Vector3.SmoothDamp(transform.position, targetDestination, ref CurrentVelocity, SmoothTime);
                    transform.rotation = Target.rotation;
                }
                catch
                {
                    throw new ArgumentException("Player has not been instantiated in scene.", "CameraFollow.cs");
                }

            }
        }
    }
}