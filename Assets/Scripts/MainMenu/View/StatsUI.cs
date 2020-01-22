using ExtraOptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Upgrade;

public class StatsUI : MonoBehaviour {

    public static StatsUI Instance { get; private set; }

    private void Start()
    {
        Instance = this;
    }


    public void InitializeStatsPanel()
    {
        CategorySelected(GameObject.Find("UI/Panels/StatsPanel/Content/CategoriesPanel/DiceButton"));
    }

    public void CategorySelected(GameObject categoryGO)
    {
        ClearStatsView();
        categoryGO.GetComponent<Image>().color = new Color(0, 0.5f, 1, 100f/256f);

        ShowViewSimple(categoryGO.GetComponentInChildren<Text>().text);
    }

    private void ShowViewSimple(string name)
    {
        Transform parentTransform = GameObject.Find("UI/Panels/StatsPanel/Content/ContentViewPanel").transform;
        string prefabPath = "Prefabs/MainMenu/Stats/" + name + "ViewPanel";
        GameObject prefab = (GameObject)Resources.Load(prefabPath, typeof(GameObject));
        //GameObject panel = 
        Instantiate(prefab, parentTransform);
    }

    private void ClearStatsView()
    {
        Transform categoryTransform = GameObject.Find("UI/Panels/StatsPanel/Content/CategoriesPanel").transform;
        foreach (Transform transform in categoryTransform.transform)
        {
            transform.GetComponent<Image>().color = new Color(0, 0.5f, 1, 0);
        }

        Transform parentTransform = GameObject.Find("UI/Panels/StatsPanel/Content/ContentViewPanel").transform;
        foreach (Transform transform in parentTransform.transform)
        {
            Destroy(transform.gameObject);
        }
    }

}
