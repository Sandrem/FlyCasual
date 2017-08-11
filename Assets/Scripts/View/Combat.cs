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
        Button closeButton = Game.PrefabsList.DiceResultsMenu.transform.Find("Confirm").GetComponent<Button>();
        Game.PrefabsList.DiceResultsMenu.SetActive(true);
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(closeAction);
    }

    public static void ShowDiceModificationButtons()
    {
        Selection.ActiveShip.GenerateAvailableActionEffectsList();

        if (Roster.GetPlayer(Selection.ActiveShip.Owner.PlayerNo).Type == Players.PlayerType.Human)
        {
            float offset = 0;
            Vector3 defaultPosition = Game.PrefabsList.DiceResultsMenu.transform.position + new Vector3(10, 210, 0);

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
        (Phases.CurrentSubPhase as SubPhases.DiceRollCombatSubPhase).ToggleConfirmDiceResultsButton(isActive);
    }

    private static void CreateDiceModificationsButton(ActionsList.GenericAction actionEffect, Vector3 position)
    {
        GameObject newButton = MonoBehaviour.Instantiate(Game.PrefabsList.GenericButton, Game.PrefabsList.DiceResultsMenu.transform);
        newButton.name = "Button" + actionEffect.EffectName;
        newButton.transform.GetComponentInChildren<Text>().text = actionEffect.EffectName;
        newButton.GetComponent<RectTransform>().position = position;
        newButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            Tooltips.EndTooltip();
            newButton.GetComponent<Button>().interactable = false;
            Selection.ThisShip.AddAlreadyExecutedActionEffect(actionEffect);
            actionEffect.ActionEffect();
        });
        Tooltips.AddTooltip(newButton, actionEffect.ImageUrl);
        newButton.GetComponent<Button>().interactable = true;
        newButton.SetActive(true);
    }


    //REMOVE
    public static void HideDiceModificationButtons()
    {
        foreach (Transform button in Game.PrefabsList.DiceResultsMenu.transform)
        {
            if (button.name.StartsWith("Button"))
            {
                MonoBehaviour.Destroy(button.gameObject);
            }
        }
        Game.PrefabsList.DiceResultsMenu.transform.Find("Confirm").gameObject.SetActive(false);
    }


    // REMOVE
    public static void HideDiceResultMenu()
    {
        Game.PrefabsList.DiceResultsMenu.SetActive(false);
        HideDiceModificationButtons();
        CurentDiceRoll.RemoveDiceModels();
    }

}
