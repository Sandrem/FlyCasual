using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public partial class DiceRerollManager
{
    private GameManagerScript Game;

    public static DiceRerollManager currentDiceRerollManager;

    public List<DiceSide> SidesCanBeRerolled;
    public int NumberOfDicesCanBeRerolled;

    public System.Action CallBack;

    public DiceRerollManager()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
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
            SidesCanBeRerolled = new List<DiceSide>
            {
                DiceSide.Blank,
                DiceSide.Focus,
                DiceSide.Success,
                DiceSide.Crit
            };
        }

        if (NumberOfDicesCanBeRerolled == 0)
        {
            NumberOfDicesCanBeRerolled = int.MaxValue;
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
        List<DiceSide> diceSides = new List<DiceSide>();

        if (SidesCanBeRerolled.Contains(DiceSide.Blank))
        {
            diceSides.Add(DiceSide.Blank);
        }

        if (SidesCanBeRerolled.Contains(DiceSide.Focus))
        {
            //if (!Selection.ActiveShip.HasToken(typeof(Tokens.FocusToken)))
            if (Combat.Attacker.GetAvailableActionEffectsList().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0)
            {
                diceSides.Add(DiceSide.Focus);
            }
        }

        Combat.CurentDiceRoll.SelectBySides(diceSides, NumberOfDicesCanBeRerolled);
    }

    private void GenerateSelectionButtons()
    {
        if (Selection.ActiveShip.Owner.GetType() == typeof(Players.HumanPlayer))
        {
            Dictionary<string, List<DiceSide>> options = new Dictionary<string, List<DiceSide>>();

            if (SidesCanBeRerolled.Contains(DiceSide.Blank))
            {
                options.Add(
                    "Select only blanks",
                    new List<DiceSide>() {
                    DiceSide.Blank
                    });
            }

            if ((SidesCanBeRerolled.Contains(DiceSide.Focus)) && (SidesCanBeRerolled.Contains(DiceSide.Blank)) && (NumberOfDicesCanBeRerolled > 1))
            {
                options.Add(
                    "Select only blanks and focuses",
                    new List<DiceSide>() {
                    DiceSide.Blank,
                    DiceSide.Focus
                    });
            }

            int offset = 0;
            foreach (var option in options)
            {
                GameObject newButton = MonoBehaviour.Instantiate(Game.PrefabsList.GenericButton, Game.PrefabsList.DiceResultsMenu.transform.Find("DiceRerollsPanel"));
                newButton.name = "Button" + option.Key;
                newButton.transform.GetComponentInChildren<Text>().text = option.Key;
                newButton.GetComponent<RectTransform>().localPosition = new Vector3(0, -offset, 0);
                newButton.GetComponent<Button>().onClick.AddListener(delegate
                {
                    SelectDicesByFilter(option.Value, NumberOfDicesCanBeRerolled);
                });
                newButton.SetActive(true);
                offset += 40;
            }
        }
    }

    private void SelectDicesByFilter(List<DiceSide> diceSides, int number)
    {
        Combat.CurentDiceRoll.SelectBySides(diceSides, number);
    }

    private void SetConfirmButtonAction()
    {
        if (Selection.ActiveShip.Owner.GetType() == typeof(Players.HumanPlayer))
        {
            Button closeButton = Game.PrefabsList.DiceResultsMenu.transform.Find("DiceRerollsPanel/Confirm").GetComponent<Button>();
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
        Game.PrefabsList.DiceResultsMenu.transform.Find("DiceModificationsPanel").gameObject.SetActive(isActive);

        if (isActive)
        {
            Combat.ToggleConfirmDiceResultsButton(true);
            Combat.ShowDiceModificationButtons();
        }
        else
        {
            Combat.HideDiceModificationButtons();
        }
    }

    private void ToggleDiceRerollsPanel(bool isActive)
    {
        Game.PrefabsList.DiceResultsMenu.transform.Find("DiceRerollsPanel").gameObject.SetActive(isActive);

        if (!isActive)
        {
            foreach (Transform button in Game.PrefabsList.DiceResultsMenu.transform.Find("DiceRerollsPanel"))
            {
                if (button.name.StartsWith("Button"))
                {
                    MonoBehaviour.Destroy(button.gameObject);
                }
            }
        }
    }

    private void ConfirmReroll()
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
