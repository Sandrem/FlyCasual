﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public partial class DiceRerollManager
{
    public static DiceRerollManager currentDiceRerollManager;

    public List<DieSide> SidesCanBeRerolled;
    public int NumberOfDiceCanBeRerolled;
    public bool IsOpposite;

    public System.Action CallBack;

    public DiceRerollManager()
    {
        currentDiceRerollManager = this;
    }

    public void Start()
    {
        OrganizeDiceView();
        CheckParameters();
        SwitchToDiceRerollsPanel();
        DoDefaultSelection();
        GenerateSelectionButtons();
        SetConfirmButtonAction();
    }

    private void OrganizeDiceView()
    {
        Combat.CurentDiceRoll.OrganizeDicePositions();
        Combat.CurentDiceRoll.ToggleRerolledLocks(true);
    }

    private void CheckParameters()
    {
        if (SidesCanBeRerolled == null)
        {
            SidesCanBeRerolled = new List<DieSide>
            {
                DieSide.Blank,
                DieSide.Focus,
                DieSide.Success,
                DieSide.Crit
            };
        }

        if (NumberOfDiceCanBeRerolled == 0)
        {
            NumberOfDiceCanBeRerolled = int.MaxValue;
        }
    }

    private void SwitchToDiceRerollsPanel(bool isReverse = false)
    {
        if (Selection.ActiveShip.Owner.GetType() == typeof(Players.HumanPlayer))
        {
            ToggleDiceModificationsPanel(isReverse);
            ToggleDiceRerollsPanel(!isReverse);
        }
    }

    private void DoDefaultSelection()
    {
        if (!IsOpposite)
        {
            DoDefaultSelectionOwnDice();
        }
        else
        {
            DoDefaultSelectionOppositeDice();
        }
    }

    private void DoDefaultSelectionOwnDice()
    {
        List<DieSide> dieSides = new List<DieSide>();

        if (SidesCanBeRerolled.Contains(DieSide.Blank))
        {
            dieSides.Add(DieSide.Blank);
        }

        if (SidesCanBeRerolled.Contains(DieSide.Focus))
        {
            //if (!Selection.ActiveShip.HasToken(typeof(Tokens.FocusToken)))
            if (Combat.Attacker.GetAvailableActionEffectsList().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0)
            {
                dieSides.Add(DieSide.Focus);
            }
        }

        Combat.CurentDiceRoll.SelectBySides(dieSides, NumberOfDiceCanBeRerolled);
    }

    private void DoDefaultSelectionOppositeDice()
    {
        List<DieSide> dieSides = new List<DieSide>();

        if (SidesCanBeRerolled.Contains(DieSide.Crit))
        {
            dieSides.Add(DieSide.Crit);
        }

        if (SidesCanBeRerolled.Contains(DieSide.Success))
        {
            dieSides.Add(DieSide.Success);
        }

        if (SidesCanBeRerolled.Contains(DieSide.Focus))
        {
            //if (!Selection.ActiveShip.HasToken(typeof(Tokens.FocusToken)))
            if ((Combat.Attacker.GetAvailableActionEffectsList().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0))
            {
                dieSides.Add(DieSide.Focus);
            }
        }

        Combat.CurentDiceRoll.SelectBySides(dieSides, NumberOfDiceCanBeRerolled);
    }

    private void GenerateSelectionButtons()
    {
        if (Selection.ActiveShip.Owner.GetType() == typeof(Players.HumanPlayer))
        {
            Dictionary<string, List<DieSide>> options = new Dictionary<string, List<DieSide>>();

            if (SidesCanBeRerolled.Contains(DieSide.Blank))
            {
                options.Add(
                    "Select only blanks",
                    new List<DieSide>() {
                    DieSide.Blank
                    });
            }

            if ((SidesCanBeRerolled.Contains(DieSide.Focus)) && (SidesCanBeRerolled.Contains(DieSide.Blank)) && (NumberOfDiceCanBeRerolled > 1))
            {
                options.Add(
                    "Select only blanks and focuses",
                    new List<DieSide>() {
                    DieSide.Blank,
                    DieSide.Focus
                    });
            }

            int offset = 0;
            foreach (var option in options)
            {
                GameObject prefab = (GameObject)Resources.Load("Prefabs/GenericButton", typeof(GameObject));
                GameObject newButton = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceRerollsPanel"));
                newButton.name = "Button" + option.Key;
                newButton.transform.GetComponentInChildren<Text>().text = option.Key;
                newButton.GetComponent<RectTransform>().localPosition = new Vector3(0, -offset, 0);
                newButton.GetComponent<Button>().onClick.AddListener(delegate
                {
                    SelectDiceByFilter(option.Value, NumberOfDiceCanBeRerolled);
                });
                newButton.SetActive(true);
                offset += 40;
            }
        }
    }

    private void SelectDiceByFilter(List<DieSide> dieSides, int number)
    {
        Combat.CurentDiceRoll.SelectBySides(dieSides, number);
    }

    private void SetConfirmButtonAction()
    {
        if (Selection.ActiveShip.Owner.GetType() == typeof(Players.HumanPlayer))
        {
            Button closeButton = GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceRerollsPanel/Confirm").GetComponent<Button>();
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(ConfirmReroll);
            closeButton.gameObject.SetActive(true);
        }
        else
        {
            ConfirmReroll();
        }            
    }

    private void ToggleDiceModificationsPanel(bool isActive)
    {
        GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel").gameObject.SetActive(isActive);

        if (isActive)
        {
            Combat.ToggleConfirmDiceResultsButton(true);
            if (!IsOpposite)
            {
                Combat.ShowDiceModificationButtons();
            }
            else
            {
                Combat.ShowOppositeDiceModificationButtons();
            }
        }
        else
        {
            Combat.HideDiceModificationButtons();
        }
    }

    private void ToggleDiceRerollsPanel(bool isActive)
    {
        GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceRerollsPanel").gameObject.SetActive(isActive);

        if (!isActive)
        {
            foreach (Transform button in GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceRerollsPanel"))
            {
                if (button.name.StartsWith("Button"))
                {
                    MonoBehaviour.Destroy(button.gameObject);
                }
            }
        }
    }

    public void ConfirmReroll()
    {
        if (Selection.ActiveShip.Owner.GetType() == typeof(Players.HumanPlayer)) BlockButtons();
        Combat.CurentDiceRoll.RerollSelected(UnblockButtons);
    }

    private void BlockButtons()
    {
        ToggleDiceRerollsPanel(false);
    }

    private void UnblockButtons(DiceRoll diceRoll)
    {
        DiceRerollManager.currentDiceRerollManager = null;

        Combat.CurentDiceRoll.ToggleRerolledLocks(false);
        if (Selection.ActiveShip.Owner.GetType() == typeof(Players.HumanPlayer)) ToggleDiceModificationsPanel(true);

        if (CallBack!=null) CallBack();
    }

}
