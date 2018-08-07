using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameModes;
using ActionsList;
using UnityEngine.Events;

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
                CloseButtonEffect = CompareResultsAndDealDamageClient;
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
            Vector3 defaultPosition = GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel").position;

            foreach (var actionEffect in Selection.ActiveShip.GetDiceModificationsGenerated())
            {
                AvailableDecisions.Add(actionEffect.Name, actionEffect);

                Vector3 position = defaultPosition + new Vector3(0, -offset, 0);
                CreateDiceModificationsButton(actionEffect, position);
                offset += 40;
            }

            ShowCloseButton(CloseButtonEffect);
            ShowDiceResultsPanel();
        }
        else
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
    }

    public static void CompareResultsAndDealDamage()
    {
        GameMode.CurrentGameMode.CompareResultsAndDealDamage();
    }

    private static void SwitchToRegularDiceModifications()
    {
        GameMode.CurrentGameMode.SwitchToRegularDiceModifications();
    }

    private static void SwitchToAfterRolledDiceModifications()
    {
        GameMode.CurrentGameMode.SwitchToAfterRolledDiceModifications();
    }

    public static void SwitchToRegularDiceModificationsClient()
    {
        HideDiceModificationButtons();
        ToggleConfirmDiceResultsButton(false);

        Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Attacker : Defender;
        Phases.CurrentSubPhase.RequiredPlayer = Selection.ActiveShip.Owner.PlayerNo;
        Selection.ActiveShip.Owner.UseDiceModifications(DiceModificationTimingType.Normal);
    }

    public static void SwitchToAfterRolledDiceModificationsClient()
    {
        HideDiceModificationButtons();
        ToggleConfirmDiceResultsButton(false);

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
        newButton.GetComponent<RectTransform>().position = position;
        newButton.GetComponent<Button>().onClick.AddListener(
            delegate { GameMode.CurrentGameMode.UseDiceModification(actionEffect.DiceModificationName); }
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

        diceModification.ActionEffect(delegate { ReGenerateListOfButtons(diceModification.DiceModificationTiming); });
    }

    private static void ReGenerateListOfButtons(DiceModificationTimingType timingType)
    {
        ShowDiceModificationButtons(timingType, true);
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
                if (Combat.Attacker.CallTryConfirmDiceResults()) ConfirmAttackDiceResults();
                break;
            case CombatStep.Defence:
                if (Combat.Defender.CallTryConfirmDiceResults()) ConfirmDefenceDiceResults();
                break;
            case CombatStep.CompareResults:
                CompareResultsAndDealDamageClient();
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
