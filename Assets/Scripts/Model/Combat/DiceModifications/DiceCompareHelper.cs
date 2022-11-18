using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DiceCompareHelper
{
    public DiceRoll AttackDiceroll { get; private set; }
    private GameObject helperPanel;
    private GameObject iconPrefabHit;
    private GameObject iconPrefabCrit;
    private int IconsCount;

    public static DiceCompareHelper currentDiceCompareHelper;

    private List<GameObject> DiceIcons = new List<GameObject>();

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

        GenerateIcons();
    }

    private void GenerateIcons()
    {
        IconsCount = 0;
        DiceIcons = new List<GameObject>();

        foreach (Transform item in helperPanel.transform.Find("DiceImages").transform)
        {
            if (item.gameObject.name == "Hit" || item.gameObject.name == "Crit") GameObject.Destroy(item.gameObject);
        }

        if (!AttackDiceroll.CancelCritsFirst)
        {
            CreateIcons(AttackDiceroll.DiceList.Where(n => n.Side == DieSide.Crit).ToList());
            CreateIcons(AttackDiceroll.DiceList.Where(n => n.Side == DieSide.Success).ToList());
        }
        else
        {
            CreateIcons(AttackDiceroll.DiceList.Where(n => n.Side == DieSide.Success).ToList());
            CreateIcons(AttackDiceroll.DiceList.Where(n => n.Side == DieSide.Crit).ToList());
        }

        UpdatePanelSize();
    }

    private void CreateIcons(List<Die> dice)
    {
        foreach (var die in dice)
        {
            CreateIcon(die);
        }
    }

    private void CreateIcon(Die die)
    {
        GameObject iconPrefab = (die.Side == DieSide.Success) ? iconPrefabHit : iconPrefabCrit;
        GameObject newIcon = MonoBehaviour.Instantiate(iconPrefab, helperPanel.transform.Find("DiceImages"));
        newIcon.transform.localPosition = new Vector3(IconsCount * 100, 0, 0);
        newIcon.name = (die.Side == DieSide.Success) ? "Hit" : "Crit";
        newIcon.SetActive(true);

        if (die.IsUncancelable)
        {
            newIcon.transform.Find("Uncancellable").gameObject.SetActive(true);
        }

        DiceIcons.Add(newIcon);

        IconsCount++;
    }

    private void ToggleHelperPanel(bool isActive)
    {
        if (IconsCount == 0) isActive = false;
        helperPanel.SetActive(isActive);
    }

    private void UpdatePanelSize()
    {
        helperPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(20 + IconsCount * 100, 100);
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
        DiceRoll diceRollForTesting = new DiceRoll(DiceKind.Attack, 0, DiceRollCheckType.Virtual);
        foreach (Die realDie in AttackDiceroll.DiceList)
        {
            Die newDie = diceRollForTesting.AddDice(realDie.Side);
            newDie.IsUncancelable = realDie.IsUncancelable;
        };

        Dictionary<string, int> results = diceRollForTesting.CancelHitsByDefence(defenceDiceRoll.Successes, true); //Dry run to calculate results
        int cancelledRegularHits = results["hits"];
        int cancelledCriticalHits = results["crits"];

        GenerateIcons();

        List<GameObject> reversedDiceIcons = new List<GameObject>(DiceIcons);
        reversedDiceIcons.Reverse();

        foreach (var diceIcon in reversedDiceIcons)
        {
            ToggleDisableDice(diceIcon, true);
        }

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

    public bool IsActive()
    {
        return helperPanel != null;
    }

}

