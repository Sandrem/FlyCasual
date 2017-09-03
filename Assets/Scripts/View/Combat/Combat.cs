using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Todo: Move to different scripts by menu names

public delegate void DiceModification();

public static partial class Combat
{

    public static void ShowDiceResultMenu(UnityEngine.Events.UnityAction closeAction)
    {
        Button closeButton = GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel/Confirm").GetComponent<Button>();
        GameObject.Find("UI/CombatDiceResultsPanel").gameObject.SetActive(true);
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(closeAction);
    }

    public static void ShowDiceModificationButtons()
    {
        Selection.ActiveShip.GenerateAvailableActionEffectsList();

        if (Roster.GetPlayer(Selection.ActiveShip.Owner.PlayerNo).Type == Players.PlayerType.Human)
        {
            float offset = 0;
            Vector3 defaultPosition = GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel").position;

            foreach (var actionEffect in Selection.ActiveShip.GetAvailableActionEffectsList())
            {
                Vector3 position = defaultPosition + new Vector3(0, -offset, 0);
                CreateDiceModificationsButton(actionEffect, position);
                offset += 40;
            }
        }

        Roster.GetPlayer(Selection.ActiveShip.Owner.PlayerNo).UseDiceModifications();
    }

    public static void ToggleConfirmDiceResultsButton(bool isActive)
    {
        if (Selection.ActiveShip.Owner.GetType() == typeof(Players.HumanPlayer))
        {
            (Phases.CurrentSubPhase as SubPhases.DiceRollCombatSubPhase).ToggleConfirmDiceResultsButton(isActive);
        }
    }

    private static void CreateDiceModificationsButton(ActionsList.GenericAction actionEffect, Vector3 position)
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/GenericButton", typeof(GameObject));
        GameObject newButton = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel"));
        newButton.name = "Button" + actionEffect.EffectName;
        newButton.transform.GetComponentInChildren<Text>().text = actionEffect.EffectName;
        newButton.GetComponent<RectTransform>().position = position;
        newButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            Tooltips.EndTooltip();
            newButton.GetComponent<Button>().interactable = false;
            Selection.ActiveShip.AddAlreadyExecutedActionEffect(actionEffect);
            actionEffect.ActionEffect(delegate { });
        });
        Tooltips.AddTooltip(newButton, actionEffect.ImageUrl);
        newButton.GetComponent<Button>().interactable = true;
        newButton.SetActive(true);
    }


    //REMOVE
    public static void HideDiceModificationButtons()
    {
        foreach (Transform button in GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").Find("DiceModificationsPanel"))
        {
            if (button.name.StartsWith("Button"))
            {
                MonoBehaviour.Destroy(button.gameObject);
            }
        }
        GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").Find("DiceModificationsPanel").Find("Confirm").gameObject.SetActive(false);
    }


    // REMOVE
    public static void HideDiceResultMenu()
    {
        GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").gameObject.SetActive(false);
        HideDiceModificationButtons();
        CurentDiceRoll.RemoveDiceModels();
    }

}
