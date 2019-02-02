﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameModes;
using ActionsList;
using UnityEngine.Events;
using GameCommands;

public static partial class Combat
{
    public static Dictionary<string, GenericAction> AvailableDecisions;

    public static void ShowDiceModificationButtons(DiceModificationTimingType type, bool isForced = false)
    {
        HideDiceModificationButtons();

        UnityAction CloseButtonEffect = null;
        switch (type)
        {
            case DiceModificationTimingType.Opposite:
                Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Defender : Attacker;
                CloseButtonEffect = SwitchToAfterRolledDiceModifications;
                break;
            case DiceModificationTimingType.AfterRolled:
                Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Attacker : Defender;
                CloseButtonEffect = SwitchToRegularDiceModifications;
                break;
            case DiceModificationTimingType.Normal:
                Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Attacker : Defender;
                CloseButtonEffect = Combat.ConfirmDiceResults;
                break;
            case DiceModificationTimingType.CompareResults:
                Selection.ActiveShip = Attacker;
                CloseButtonEffect = CompareResultsAndDealDamage;
                break;
            default:
                break;
        }

        Phases.CurrentSubPhase.RequiredPlayer = Selection.ActiveShip.Owner.PlayerNo;

        AvailableDecisions = new Dictionary<string, GenericAction>();
        Selection.ActiveShip.GenerateDiceModifications(type);

        if (Selection.ActiveShip.GetDiceModificationsGenerated().Count > 0 || isForced)
        {
            float offset = 0;

            Vector3 position = Vector3.zero;
            foreach (var actionEffect in Selection.ActiveShip.GetDiceModificationsGenerated())
            {
                AvailableDecisions.Add(actionEffect.Name, actionEffect);

                position += new Vector3(0, -offset, 0);
                CreateDiceModificationsButton(actionEffect, position);

                offset = 65;
            }

            ShowCloseButton(CloseButtonEffect);

            ShowDiceResultsPanel();
        }
        else
        {
            if (Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).PlayerType != Players.PlayerType.Ai)
            {
                if (type != DiceModificationTimingType.Normal)
                {
                    CloseButtonEffect.Invoke();
                }
                else
                {
                    ShowCloseButton(CloseButtonEffect);
                    ShowDiceResultsPanel();
                }
            }
        }
    }

    private static void ShowCloseButton(UnityAction closeButtonEffect)
    {
        ToggleConfirmDiceResultsButton(true);

        Button closeButton = GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel/Confirm").GetComponent<Button>();
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(closeButtonEffect);
    }

    private static void ShowDiceResultsPanel()
    {
        GameObject.Find("UI/CombatDiceResultsPanel").gameObject.SetActive(true);
        Phases.CurrentSubPhase.IsReadyForCommands = true;
    }

    public static void SwitchToRegularDiceModifications()
    {
        HideDiceModificationButtons();
        ToggleConfirmDiceResultsButton(false);

        Attacker.ClearAlreadyUsedDiceModifications();
        Defender.ClearAlreadyUsedDiceModifications();

        Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Attacker : Defender;
        Phases.CurrentSubPhase.RequiredPlayer = Selection.ActiveShip.Owner.PlayerNo;

        Selection.ActiveShip.Owner.UseDiceModifications(DiceModificationTimingType.Normal);
    }

    public static void SwitchToAfterRolledDiceModifications()
    {
        HideDiceModificationButtons();
        ToggleConfirmDiceResultsButton(false);

        Attacker.ClearAlreadyUsedDiceModifications();
        Defender.ClearAlreadyUsedDiceModifications();

        Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Attacker : Defender;
        Phases.CurrentSubPhase.RequiredPlayer = Selection.ActiveShip.Owner.PlayerNo;

        Selection.ActiveShip.Owner.UseDiceModifications(DiceModificationTimingType.AfterRolled);
    }

    public static void ToggleConfirmDiceResultsButton(bool isActive)
    {
        if (Selection.ActiveShip.Owner.GetType() == typeof(Players.HumanPlayer))
        {
            (Phases.CurrentSubPhase as SubPhases.DiceRollCombatSubPhase).PrepareToggleConfirmButton(isActive);
        }
    }

    private static void CreateDiceModificationsButton(GenericAction actionEffect, Vector3 position)
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/GenericButton", typeof(GameObject));
        GameObject newButton = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel"));
        newButton.name = "Button" + actionEffect.DiceModificationName;
        newButton.transform.GetComponentInChildren<Text>().text = actionEffect.DiceModificationName;
        newButton.GetComponent<RectTransform>().localPosition = position;
        newButton.GetComponent<Button>().onClick.AddListener(
            delegate {
                GameCommand command = Combat.GenerateDiceModificationCommand(actionEffect.DiceModificationName);
                GameMode.CurrentGameMode.ExecuteCommand(command);
            }
        );
        Tooltips.AddTooltip(newButton, actionEffect.ImageUrl);
        newButton.GetComponent<Button>().interactable = true;

        newButton.SetActive(Selection.ActiveShip.Owner.GetType() == typeof(Players.HumanPlayer));
    }

    public static GameCommand GenerateDiceModificationCommand(string diceModificationName)
    {
        JSONObject parameters = new JSONObject();
        string diceModificationNameFixed = diceModificationName.Replace('"', '_');
        parameters.AddField("name", diceModificationNameFixed);
        return GameController.GenerateGameCommand(
            GameCommandTypes.DiceModification,
            Phases.CurrentSubPhase.GetType(),
            parameters.ToString()
        );
    }

    public static void UseDiceModification(string diceModificationName)
    {
        Phases.CurrentSubPhase.IsReadyForCommands = false;

        Tooltips.EndTooltip();

        GameObject DiceModificationButton = GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel").Find("Button" + diceModificationName).gameObject;
        DiceModificationButton.GetComponent<Button>().interactable = false;

        GenericAction diceModification = AvailableDecisions[diceModificationName];

        switch (diceModification.DiceModificationTiming)
        {
            case DiceModificationTimingType.Normal:
            case DiceModificationTimingType.AfterRolled:
                Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Attacker : Defender;
                break;
            case DiceModificationTimingType.Opposite:
                Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Defender : Attacker;
                break;
            case DiceModificationTimingType.CompareResults:
                Selection.ActiveShip = Attacker;
                break;
            default:
                break;
        }

        Selection.ActiveShip.AddAlreadyUsedDiceModification(diceModification);

        diceModification.ActionEffect( delegate {
            ReplaysManager.ExecuteWithDelay(delegate {
                ReGenerateListOfButtons(diceModification.DiceModificationTiming);
            });
        });
    }

    private static void ReGenerateListOfButtons(DiceModificationTimingType timingType)
    {
        ShowDiceModificationButtons(timingType, true);
        Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).UseDiceModifications(timingType);
    }

    public static void ConfirmDiceResults()
    {
        GameCommand command = Combat.GenerateDiceModificationCommand("OK");
        GameMode.CurrentGameMode.ExecuteCommand(command);
    }

    public static void ConfirmDiceResultsClient()
    {
        Phases.CurrentSubPhase.IsReadyForCommands = false;

        switch (AttackStep)
        {
            case CombatStep.Attack:
                if (Combat.Attacker.CallTryConfirmDiceResults()) ReplaysManager.ExecuteWithDelay(ConfirmAttackDiceResults);
                break;
            case CombatStep.Defence:
                if (Combat.Defender.CallTryConfirmDiceResults()) ReplaysManager.ExecuteWithDelay(ConfirmDefenceDiceResults);
                break;
            case CombatStep.CompareResults:
                ReplaysManager.ExecuteWithDelay(CompareResultsAndDealDamage);
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
