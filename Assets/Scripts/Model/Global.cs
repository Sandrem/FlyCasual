using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Global : MonoBehaviour {

    public static Global Instance;

    private static bool isAlreadyInitialized;

    public static string test = "I am accessible from every scene";

    public static string CurrentVersion = "0.4.4 Hotfix 2";
    public static int CurrentVersionInt = 100040402;

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
        ToggelLoadingScreen(false);
        Phases.StartPhases();
    }

    public static void ToggelLoadingScreen(bool isActive)
    {
        Transform loadingScreen = GameObject.Find("GlobalUI").transform.Find("OpponentSquad");
        loadingScreen.GetComponent<Image>().sprite = MainMenu.GetRandomBackground();
        if (loadingScreen != null) loadingScreen.gameObject.SetActive(isActive);
    }

}
