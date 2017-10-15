using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Todo: Move to different scripts by menu names

public delegate void DiceModification();

public static partial class Combat
{

    public static void ShowOwnDiceResultMenu()
    {
        ShowDiceModificationButtons();
    }

    public static void ShowOppositeDiceResultMenu()
    {
        ShowOppositeDiceModificationButtons();
    }

    public static void ShowOppositeDiceModificationButtons()
    {
        Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Defender : Attacker;

        ToggleConfirmDiceResultsButton(true);

        Selection.ActiveShip.GenerateAvailableOppositeActionEffectsList();

        if (Selection.ActiveShip.GetAvailableOppositeActionEffectsList().Count > 0)
        {

            float offset = 0;
            Vector3 defaultPosition = GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel").position;

            foreach (var oppositeActionEffect in Selection.ActiveShip.GetAvailableOppositeActionEffectsList())
            {
                Vector3 position = defaultPosition + new Vector3(0, -offset, 0);
                CreateDiceModificationsButton(oppositeActionEffect, position);
                offset += 40;
            }

            ToggleConfirmDiceResultsButton(true);

            Button closeButton = GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel/Confirm").GetComponent<Button>();
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(SwitchToOwnDiceModifications);

            GameObject.Find("UI/CombatDiceResultsPanel").gameObject.SetActive(true);
        }
        else
        {
            ToggleConfirmDiceResultsButton(false);

            Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Attacker : Defender;
            Selection.ActiveShip.Owner.UseOwnDiceModifications();
        }
    }

    private static void SwitchToOwnDiceModifications()
    {
        HideDiceModificationButtons();
        ToggleConfirmDiceResultsButton(false);

        Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Attacker : Defender;
        Selection.ActiveShip.Owner.UseOwnDiceModifications();
    }

    public static void ShowDiceModificationButtons()
    {
        Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Attacker : Defender;

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

            ToggleConfirmDiceResultsButton(true);
        }

        Button closeButton = GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel/Confirm").GetComponent<Button>();
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(Combat.ConfirmDiceResults);

        GameObject.Find("UI/CombatDiceResultsPanel").gameObject.SetActive(true);
    }

    public static void ToggleConfirmDiceResultsButton(bool isActive)
    {
        if (Selection.ActiveShip.Owner.GetType() == typeof(Players.HumanPlayer))
        {
            (Phases.CurrentSubPhase as SubPhases.DiceRollCombatSubPhase).PrepareToggleConfirmButton(isActive);
        }
    }

    private static void CreateDiceModificationsButton(ActionsList.GenericAction actionEffect, Vector3 position)
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/GenericButton", typeof(GameObject));
        GameObject newButton = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel"));
        newButton.name = "Button" + actionEffect.EffectName;
        newButton.transform.GetComponentInChildren<Text>().text = actionEffect.EffectName;
        newButton.GetComponent<RectTransform>().position = position;
        newButton.GetComponent<Button>().onClick.AddListener(
            delegate
            {
                if (!Network.IsNetworkGame)
                {
                    UseDiceModification(newButton, actionEffect);
                }
                else
                {
                    Network.ShowMessage(actionEffect.EffectName);
                }
            }
        );
        Tooltips.AddTooltip(newButton, actionEffect.ImageUrl);
        newButton.GetComponent<Button>().interactable = true;
        newButton.SetActive(true);
    }

    private static void UseDiceModification(GameObject newButton, ActionsList.GenericAction actionEffect)
    {
        Tooltips.EndTooltip();
        newButton.GetComponent<Button>().interactable = false;
        if (!actionEffect.IsOpposite)
        {
            Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Attacker : Defender;
            Selection.ActiveShip.AddAlreadyExecutedActionEffect(actionEffect);
        }
        else
        {
            Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Defender : Attacker;
            Selection.ActiveShip.AddAlreadyExecutedOppositeActionEffect(actionEffect);
        }
        actionEffect.ActionEffect(delegate { });
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
        ToggleConfirmDiceResultsButton(false);
    }


    // REMOVE
    public static void HideDiceResultMenu()
    {
        GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").gameObject.SetActive(false);
        HideDiceModificationButtons();
        CurentDiceRoll.RemoveDiceModels();
    }

}
