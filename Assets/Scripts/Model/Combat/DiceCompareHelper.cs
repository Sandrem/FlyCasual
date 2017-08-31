using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceCompareHelper
{
    public DiceRoll AttackDiceroll { get; private set; }
    private GameObject helperPanel;
    private GameObject iconPrefabHit;
    private GameObject iconPrefabCrit;
    private int iconsCount;

    public static DiceCompareHelper currentDiceCompareHelper;

    private List<GameObject> diceIcons = new List<GameObject>();

    public DiceCompareHelper(DiceRoll attackDiceroll)
    {
        AttackDiceroll = attackDiceroll;
        currentDiceCompareHelper = this;

        GenerateHelper();
        ToggleHelperPanel(true);
    }

    private void GenerateHelper()
    {
        helperPanel = GameObject.Find("UI/CombatDiceResultsPanel/DiceCompareHelp");
        helperPanel = GameObject.Find("UI").gameObject.transform.Find("CombatDiceResultsPanel").gameObject.transform.Find("DiceCompareHelp").gameObject;
        iconPrefabHit = helperPanel.transform.Find("DiceImages").gameObject.transform.Find("AttackHit").gameObject;
        iconPrefabCrit = helperPanel.transform.Find("DiceImages").gameObject.transform.Find("AttackCrit").gameObject;

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
        newIcon.name = (diceSide == DiceSide.Success) ? "Hit" : "Crit";
        newIcon.SetActive(true);

        diceIcons.Add(newIcon);

        iconsCount++;
    }

    private void ToggleHelperPanel(bool isActive)
    {
        if (iconsCount == 0) isActive = false;
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
        currentDiceCompareHelper = null;
    }

    public void ShowCancelled(DiceRoll defenceDiceRoll)
    {
        int cancelledRegularHits = 0;
        int cancelledCriticalHits = 0;

        int cancelsNum = defenceDiceRoll.Successes;

        int regularHits = AttackDiceroll.RegularSuccesses;
        cancelledRegularHits = (cancelsNum > regularHits) ? regularHits: cancelsNum;
        cancelsNum = cancelsNum - cancelledRegularHits;

        if (cancelsNum > 0)
        {
            int criticalHits = AttackDiceroll.CriticalSuccesses;
            cancelledCriticalHits = (cancelsNum > criticalHits) ? criticalHits : cancelsNum;
            cancelsNum = cancelsNum - cancelledCriticalHits;
        }

        List<GameObject> reversedDiceIcons = new List<GameObject>(diceIcons);
        reversedDiceIcons.Reverse();

        foreach (var diceIcon in reversedDiceIcons)
        {
            switch (diceIcon.name)
            {
                case "Hit":
                    if (cancelledRegularHits > 0)
                    {
                        ToggleDisableDice(diceIcon, false);
                        cancelledRegularHits--;
                    };
                    break;
                case "Crit":
                    if (cancelledCriticalHits > 0)
                    {
                        ToggleDisableDice(diceIcon, false);
                        cancelledCriticalHits--;
                    };
                    break;
                default:
                    break;
            }
        }
    }

    private void ToggleDisableDice(GameObject diceIcon, bool isActive)
    {
        Color currentColor = diceIcon.GetComponent<Image>().color;
        diceIcon.GetComponent<Image>().color = new Color
        {
            r = currentColor.r,
            g = currentColor.g,
            b = currentColor.b,
            a = (isActive) ? 1 : 0.25f
        };
    }

}

