using UnityEngine;
using System.Collections;
using System;

public class CameraFollow : MonoBehaviour
{
    private readonly float SmoothTime = 0.15f;
    private Vector3 CurrentVelocity = Vector3.zero;
    internal static Vector3 offset;
    private GameObject player;

    // Update is called once per frame
    void Update()
    {
        if (Menu.GameStarted)
        {
            if (player != null)
            {
                if (Cursor.lockState != CursorLockMode.Confined)
                {
                    Cursor.lockState = CursorLockMode.Confined;
                }
                //RaycastHit hit;
                Vector3 targPos;
                //if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
               // {
                //    targPos = (hit.point + player.transform.position) / 2;
                //}
                //else
               // {
                    targPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
               // }
                targPos.y = transform.position.y;
                transform.position = Vector3.SmoothDamp(transform.position, targPos, ref CurrentVelocity, SmoothTime);
                transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
                offset = Camera.main.WorldToScreenPoint(player.transform.position - transform.position);
            }
            else
            {
                player = GameObject.Find("Player");
            }
        }
    }
}