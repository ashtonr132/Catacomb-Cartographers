using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    internal bool isInput = false;
    private GameObject[] MenuButtons;
    private int MenuLength;
    private int LevelSize, Difficulty;
    private int? Seed = null;
    private List<InputField> SkirmishIFS;
    internal static bool GameStarted = false;
    [SerializeField]
    GameObject Level;

    // Start is called before the first frame update
    void Start()
    {
        MenuLength = transform.childCount;
        MenuButtons = new GameObject[MenuLength];
        SkirmishIFS = new List<InputField>();
        for (int i = 0; i < MenuLength; i++)
        {
            MenuButtons[i] = transform.GetChild(i).gameObject;
        }

        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { EnableSubMenu(0); });
        transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { EnableSubMenu(1); });
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { EnableSubMenu(2); });
        transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { EnableSubMenu(3); });

        foreach (Transform t in transform.GetChild(1)) //skirmish input fields
        {
            if (t.GetComponent<InputField>() != null)
            {
                SkirmishIFS.Add(t.GetComponent<InputField>());
            }
        }

        transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { transform.parent.GetComponent<GameFiles>().setMainSave(1); });
        transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { transform.parent.GetComponent<GameFiles>().setMainSave(2); });
        transform.GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { transform.parent.GetComponent<GameFiles>().setMainSave(3); });

        transform.GetChild(1).GetChild(4).GetComponent<Button>().onClick.AddListener(StartSkirmish);
        transform.GetChild(3).GetChild(1).GetComponent<Button>().onClick.AddListener(exitGame);
        EnableSubMenu(); //disable all sub menues after listeners setup
    }

    internal void EnableSubMenu(int? Menu = null)
    {
        foreach (GameObject g in MenuButtons)
        {
            if (Menu != null && g == MenuButtons[Menu.Value])
            {
                InvertActiveMenu(g.transform, true);
            }
            else
            {
                InvertActiveMenu(g.transform);
            }
        }
    }

    internal void InvertActiveMenu(Transform M, bool active = false)
    {
        foreach (Transform t in M)
        {
            if (t.name != "Text")
            {
                if (t.gameObject.activeSelf != active)
                {
                    t.gameObject.SetActive(active);
                }
            }
        }
    }

    private void StartSkirmish()
    {
        if (SkirmishIFS[0].text != "")
        {
            LevelSize = int.Parse(SkirmishIFS[0].text);
        }
        if (SkirmishIFS[1].text != "")
        {
            Seed = int.Parse(SkirmishIFS[0].text);
        }
        if (SkirmishIFS[2].text != "")
        {
            Difficulty = int.Parse(SkirmishIFS[0].text);
        }
        Level.GetComponent<LevelGen>().InitLevel(LevelSize, Seed, Difficulty, true);
        gameStarted();
    }

    internal void gameStarted()
    {
        GameStarted = true;
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }
    }

    internal void inGameSettings()
    {
        EnableSubMenu();
        bool flip;
        if (transform.GetChild(2).gameObject.activeSelf)
        {
            flip = false;
        }
        else
        {
            flip = true;
        }
        transform.GetChild(2).gameObject.SetActive(flip);
        transform.GetChild(3).gameObject.SetActive(flip);
    }

    internal void exitGame()
    {
        if (GameStarted)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
            Level.GetComponent<MeshFilter>().mesh = null;
            Destroy(Level.GetComponent<MeshCollider>());
            foreach (Transform t in Level.transform)
            {
                if(t.name != "Walls" && t.name != "Roof")
                {
                    Destroy(t.gameObject);
                }
                else
                {
                    t.GetComponent<MeshFilter>().mesh = null;
                    Destroy(t.GetComponent<MeshCollider>());
                }
            }
            EnableSubMenu();
        }
        else
        {
            Application.Quit();
        }
    }
}
