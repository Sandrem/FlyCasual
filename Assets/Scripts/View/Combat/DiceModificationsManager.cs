using ActionsList;
using GameCommands;
using GameModes;
using Ship;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class DiceModificationsManager
{
    public void HideDiceModificationsButtonsList()
    {
        foreach (Transform transform in GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").Find("DiceModificationsPanel").transform)
        {
            if (transform.name != "Confirm") GameObject.Destroy(transform.gameObject);
        }
        HideOkButton();
    }

    public void ShowDiceModificationsUi()
    {
        GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").gameObject.SetActive(true);
        HideDiceModificationsButtonsList();
    }

    public void HideDiceModificationsUi()
    {
        GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").gameObject.SetActive(false);
    }

    public void ShowDiceModificationButtons(List<GenericAction> diceModifications)
    {
        GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").gameObject.SetActive(true);

        HideDiceModificationsButtonsList();

        float offset = 0;

        Vector3 position = Vector3.zero;
        foreach (var actionEffect in Selection.ActiveShip.GetDiceModificationsGenerated())
        {
            AvailableDiceModifications.Add(actionEffect.Name, actionEffect);

            position += new Vector3(0, -offset, 0);
            CreateDiceModificationsButton(actionEffect, position);

            offset = 65;
        }

        ShowOkButton();
    }

    public static void CreateDiceModificationsButton(GenericAction actionEffect, Vector3 position)
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/GenericButton", typeof(GameObject));
        GameObject newButton = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel"));
        newButton.name = "Button" + actionEffect.DiceModificationName;
        newButton.transform.GetComponentInChildren<Text>().text = actionEffect.DiceModificationName;
        newButton.GetComponent<RectTransform>().localPosition = position;
        newButton.GetComponent<Button>().onClick.AddListener(
            delegate {
                GameCommand command = DiceModificationsManager.GenerateDiceModificationCommand(actionEffect.DiceModificationName);
                GameMode.CurrentGameMode.ExecuteCommand(command);
            }
        );
        Tooltips.AddTooltip(newButton, actionEffect.ImageUrl);
        newButton.GetComponent<Button>().interactable = true;

        newButton.SetActive(Selection.ActiveShip.Owner.GetType() == typeof(Players.HumanPlayer));
    }

    private void ShowOkButton()
    {
        Button closeButton = GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").Find("DiceModificationsPanel").Find("Confirm").GetComponent<Button>();
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(
            delegate {
                GameCommand command = DiceModificationsManager.GenerateDiceModificationCommand("OK");
                GameMode.CurrentGameMode.ExecuteCommand(command);
            }
        );
        closeButton.gameObject.SetActive(true);
    }

    private void HideOkButton()
    {
        Button closeButton = GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").Find("DiceModificationsPanel").Find("Confirm").GetComponent<Button>();
        closeButton.gameObject.SetActive(false);
    }
}
