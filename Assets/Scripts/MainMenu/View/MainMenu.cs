using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;
using System.Linq;
using Players;

public partial class MainMenu : MonoBehaviour {

    public GameObject RosterBuilderPrefab;
    public GameObject UpgradeLinePrefab;

    public GameObject CurrentPanel;

    private void SetPositions()
    {
        GameObject.Find("UI/Panels/MainMenuPanel").transform.position = new Vector3(Screen.width / 20, Screen.height - Screen.height / 20, 0.0f);
        GameObject.Find("UI/BackgroundImage").GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.height * 16f / 9f, Screen.height);
    }

    private void SetCurrentPanel()
    {
        CurrentPanel = GameObject.Find("UI/Panels/MainMenuPanel");
    }

    public void ChangePanel(GameObject panel)
    {
        CurrentPanel.SetActive(false);
        InitializePanelContent(panel.name);
        panel.SetActive(true);
        CurrentPanel = panel;
    }

    private void InitializePanelContent(string panelName)
    {
        switch (panelName)
        {
            case "RosterBuilderPanel": RosterBuilder.Initialize();
                break;
        }
    }

}
