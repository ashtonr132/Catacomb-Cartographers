using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNums : MonoBehaviour
{
    private static GameObject canvas, textParent;
    internal static void CreateDamageText(string text, Vector3 location)
    {
        if (canvas == null)
        {
            canvas = GameObject.Find("Canvas");
        }
        if (textParent == null)
        {
            textParent = Resources.Load<GameObject>("HPopParent");
        }
        var nl = Camera.main.WorldToScreenPoint(location + new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), 0));
        GameObject newtp = Instantiate(textParent, nl, Quaternion.identity, canvas.transform);
        newtp.GetComponentInChildren<Text>().text = text;
        Destroy(newtp, 0.75f);
    }

}
