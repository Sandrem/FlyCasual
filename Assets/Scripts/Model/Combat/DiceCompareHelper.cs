using System;
using System.Collections.Generic;
using UnityEngine;

public class DiceCompareHelper
{
    public DiceRoll AttackDiceroll { get; private set; }
    private GameObject helperPanel;
    private GameObject iconPrefabHit;
    private GameObject iconPrefabCrit;
    private int iconsCount;

    public static DiceCompareHelper currentDiceCompareHelper;

    public DiceCompareHelper(DiceRoll attackDiceroll)
    {
        AttackDiceroll = attackDiceroll;
        currentDiceCompareHelper = this;

        GenerateHelper();
        ToggleHelperPanel(true);
    }

    private void GenerateHelper()
    {
        helperPanel = GameObject.Find("UI/DiceResultsPanel/DiceCompareHelp");
        helperPanel = GameObject.Find("UI").gameObject.transform.Find("DiceResultsPanel").gameObject.transform.Find("DiceCompareHelp").gameObject;
        iconPrefabHit = helperPanel.transform.Find("DiceImages").gameObject.transform.Find("AttackHit").gameObject;
        iconPrefabCrit = helperPanel.transform.Find("DiceImages").gameObject.transform.Find("AttackHit").gameObject;

        CreateIcons(DiceSide.Crit, AttackDiceroll.CriticalSuccesses);
        CreateIcons(DiceSide.Success, AttackDiceroll.RegularSuccesses);

        UpdatePanelSize();
    }

    private void CreateIcons(DiceSide diceSide, int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreateIcon(diceSide);
        }
    }

    private void CreateIcon(DiceSide diceSide)
    {
        GameObject iconPrefab = (diceSide == DiceSide.Success) ? iconPrefabHit : iconPrefabCrit;
        GameObject newIcon = MonoBehaviour.Instantiate(iconPrefab, helperPanel.transform.Find("DiceImages"));
        newIcon.transform.localPosition = new Vector3(iconsCount * 100, 0, 0);
        newIcon.SetActive(true);
        iconsCount++;
    }

    private void ToggleHelperPanel(bool isActive)
    {
        helperPanel.SetActive(isActive);
    }

    private void UpdatePanelSize()
    {
        helperPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(20 + iconsCount * 100, 100);
    }

    public void Close()
    {
        foreach (Transform transform in helperPanel.transform.Find("DiceImages"))
        {
            if (transform.gameObject.activeSelf)
            {
                MonoBehaviour.Destroy(transform.gameObject);
            }
        }
        ToggleHelperPanel(false);
    }

}

