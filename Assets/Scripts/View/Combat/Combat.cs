using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameModes;

public static partial class Combat
{

    public static Dictionary<string, ActionsList.GenericAction> AvailableDecisions;

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
        Phases.CurrentSubPhase.RequiredPlayer = Selection.ActiveShip.Owner.PlayerNo;

        ToggleConfirmDiceResultsButton(true);

        AvailableDecisions = new Dictionary<string, ActionsList.GenericAction>();
        Selection.ActiveShip.GenerateAvailableOppositeActionEffectsList();

        if (Selection.ActiveShip.GetAvailableOppositeActionEffectsList().Count > 0)
        {

            float offset = 0;
            Vector3 defaultPosition = GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel").position;

            foreach (var oppositeActionEffect in Selection.ActiveShip.GetAvailableOppositeActionEffectsList())
            {
                AvailableDecisions.Add(oppositeActionEffect.Name, oppositeActionEffect);

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
            SwitchToOwnDiceModificationsClient();
        }
    }

    private static void SwitchToOwnDiceModifications()
    {
        GameMode.CurrentGameMode.SwitchToOwnDiceModifications();
    }

    public static void SwitchToOwnDiceModificationsClient()
    {
        HideDiceModificationButtons();
        ToggleConfirmDiceResultsButton(false);

        Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Attacker : Defender;
        Phases.CurrentSubPhase.RequiredPlayer = Selection.ActiveShip.Owner.PlayerNo;
        Selection.ActiveShip.Owner.UseOwnDiceModifications();
    }

    public static void ShowDiceModificationButtons()
    {
        Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Attacker : Defender;

        AvailableDecisions = new Dictionary<string, ActionsList.GenericAction>();
        Selection.ActiveShip.GenerateAvailableActionEffectsList();

        float offset = 0;
        Vector3 defaultPosition = GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel").position;

        foreach (var actionEffect in Selection.ActiveShip.GetAvailableActionEffectsList())
        {
            AvailableDecisions.Add(actionEffect.Name, actionEffect);

            Vector3 position = defaultPosition + new Vector3(0, -offset, 0);
            CreateDiceModificationsButton(actionEffect, position);
            offset += 40;
        }

        ToggleConfirmDiceResultsButton(true);

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
            delegate { GameMode.CurrentGameMode.UseDiceModification(actionEffect.EffectName); }
        );
        Tooltips.AddTooltip(newButton, actionEffect.ImageUrl);
        newButton.GetComponent<Button>().interactable = true;

        newButton.SetActive(Selection.ActiveShip.Owner.GetType() == typeof(Players.HumanPlayer));
    }

    public static void UseDiceModification(string diceModificationName)
    {
        Tooltips.EndTooltip();

        GameObject DiceModificationButton = GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel").Find("Button" + diceModificationName).gameObject;
        DiceModificationButton.GetComponent<Button>().interactable = false;

        ActionsList.GenericAction diceModification = AvailableDecisions[diceModificationName];

        if (!diceModification.IsOpposite)
        {
            Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Attacker : Defender;
            Selection.ActiveShip.AddAlreadyExecutedActionEffect(diceModification);
        }
        else
        {
            Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Defender : Attacker;
            Selection.ActiveShip.AddAlreadyExecutedOppositeActionEffect(diceModification);
        }

        //TODO: Re-generate list instead
        diceModification.ActionEffect(delegate { });
    }

    public static void ConfirmDiceResults()
    {
        GameMode.CurrentGameMode.ConfirmDiceResults();
    }

    public static void ConfirmDiceResultsClient()
    {
        switch (AttackStep)
        {
            case CombatStep.Attack:
                ConfirmAttackDiceResults();
                break;
            case CombatStep.Defence:
                ConfirmDefenceDiceResults();
                break;
        }
    }

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


    public static void HideDiceResultMenu()
    {
        GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").gameObject.SetActive(false);
        HideDiceModificationButtons();
        CurrentDiceRoll.RemoveDiceModels();
    }

}
