using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class DiceRerollManager
{
    private GameManagerScript Game;

    public List<DiceSide> SidesCanBeRerolled;
    public int NumberOfDicesCanBeRerolled;

    public DiceRerollManager()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    public void Start()
    {
        SwitchToDiceRerollsPanel();
        GenerateSelectionButtons();
        SetConfirmButtonAction();
    }

    private void SwitchToDiceRerollsPanel(bool isReverse = false)
    {
        ToggleDiceModificationsPanel(isReverse);
        ToggleDiceRerollsPanel(!isReverse);
    }

    private void GenerateSelectionButtons()
    {
        // TODO: Generate different buttons

        GameObject newButton = MonoBehaviour.Instantiate(Game.PrefabsList.GenericButton, Game.PrefabsList.DiceResultsMenu.transform.Find("DiceRerollsPanel"));
        newButton.name = "ButtonBlanks";
        newButton.transform.GetComponentInChildren<Text>().text = "Select blanks";
        newButton.GetComponent<RectTransform>().localPosition = Vector3.zero;
        newButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            SelectDicesByFilter(DiceSide.Blank, int.MaxValue);
        });
        newButton.SetActive(true);
    }

    private void SelectDicesByFilter(DiceSide diceSide, int number)
    {
        //TODO: Select dices
    }

    private void SetConfirmButtonAction()
    {
        Button closeButton = Game.PrefabsList.DiceResultsMenu.transform.Find("DiceRerollsPanel/Confirm").GetComponent<Button>();
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(ConfirmReroll);
        closeButton.gameObject.SetActive(true);
    }

    private void ToggleDiceModificationsPanel(bool isActive)
    {
        Game.PrefabsList.DiceResultsMenu.transform.Find("DiceModificationsPanel").gameObject.SetActive(isActive);

        if (isActive)
        {
            Combat.ShowDiceModificationButtons();
            Combat.ToggleConfirmDiceResultsButton(true);
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
        Messages.ShowInfo("DONE!");
        BlockButtons();

        //TODO: Call roll only for selected
        DicesManager.RerollDices(Combat.CurentDiceRoll, "blank", UnblockButtons);
    }

    private void BlockButtons()
    {
        ToggleDiceRerollsPanel(false);
    }

    private void UnblockButtons(DiceRoll diceRoll)
    {
        ToggleDiceModificationsPanel(true);
    }

}
