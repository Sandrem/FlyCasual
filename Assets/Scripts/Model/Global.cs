using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour {

    public static Global Instance;

    private static bool isAlreadyInitialized;

    public static string test = "I am accessible from every scene";

    public static string CurrentVersion = "0.4.3.1";
    public static int CurrentVersionInt = 100040310;

    void Awake()
    {
        if (!isAlreadyInitialized)
        {
            Instance = this;
            isAlreadyInitialized = true;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        Tooltips.CheckTooltip();
    }

    public static void StartBattle()
    {
        HideOpponentSquad();
        Phases.StartPhases();
    }

    private static void HideOpponentSquad()
    {
        Transform opponentSquad = GameObject.Find("GlobalUI").transform.Find("OpponentSquad");
        if (opponentSquad != null) opponentSquad.gameObject.SetActive(false);
    }

}
