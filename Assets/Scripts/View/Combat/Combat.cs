using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameModes;
using ActionsList;

public static partial class Combat
{

    public static Dictionary<string, GenericAction> AvailableDecisions;

    public static void ShowOwnDiceResultMenu()
    {
        ShowDiceModificationButtons();
    }

    public static void ShowOppositeDiceResultMenu()
    {
        ShowOppositeDiceModificationButtons();
    }

    public static void ShowCompareResultsMenu()
    {
        HideDiceModificationButtons();
        ShowCompareResultsButtons();
    }

    public static void ShowOppositeDiceModificationButtons(bool isForced = false)
    {
        Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Defender : Attacker;
        Phases.CurrentSubPhase.RequiredPlayer = Selection.ActiveShip.Owner.PlayerNo;

        ToggleConfirmDiceResultsButton(true);

        AvailableDecisions = new Dictionary<string, GenericAction>();
        Selection.ActiveShip.GenerateAvailableOppositeActionEffectsList();

        if (Selection.ActiveShip.GetAvailableOppositeActionEffectsList().Count > 0 || isForced)
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

    public static void ShowCompareResultsButtons(bool isForced = false)
    {
        Selection.ActiveShip = Attacker;
        Phases.CurrentSubPhase.RequiredPlayer = Selection.ActiveShip.Owner.PlayerNo;

        ToggleConfirmDiceResultsButton(true);

        AvailableDecisions = new Dictionary<string, GenericAction>();
        Selection.ActiveShip.GenerateAvailableCompareResultsEffectsList();

        if (Selection.ActiveShip.GetAvailableCompareResultsEffectsList().Count > 0 || isForced)
        {
            float offset = 0;
            Vector3 defaultPosition = GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel").position;

            foreach (var compareResultsEffect in Selection.ActiveShip.GetAvailableCompareResultsEffectsList())
            {
                AvailableDecisions.Add(compareResultsEffect.Name, compareResultsEffect);

                Vector3 position = defaultPosition + new Vector3(0, -offset, 0);
                CreateDiceModificationsButton(compareResultsEffect, position);
                offset += 40;
            }

            ToggleConfirmDiceResultsButton(true);

            Button closeButton = GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel/Confirm").GetComponent<Button>();
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CompareResultsAndDealDamage);

            GameObject.Find("UI/CombatDiceResultsPanel").gameObject.SetActive(true);
        }
        else
        {
            CompareResultsAndDealDamageClient();
        }
    }

    public static void CompareResultsAndDealDamage()
    {
        GameMode.CurrentGameMode.CompareResultsAndDealDamage();
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

        AvailableDecisions = new Dictionary<string, GenericAction>();
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

    private static void CreateDiceModificationsButton(GenericAction actionEffect, Vector3 position)
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

        GenericAction diceModification = AvailableDecisions[diceModificationName];

        switch (diceModification.DiceModificationTiming)
        {
            case DiceModificationTimingType.Normal:
                Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Attacker : Defender;
                Selection.ActiveShip.AddAlreadyExecutedActionEffect(diceModification);
                break;
            case DiceModificationTimingType.Opposite:
                Selection.ActiveShip = (AttackStep == CombatStep.Attack) ? Defender : Attacker;
                Selection.ActiveShip.AddAlreadyExecutedOppositeActionEffect(diceModification);
                break;
            case DiceModificationTimingType.CompareResults:
                Selection.ActiveShip = Attacker;
                Selection.ActiveShip.AddAlreadyExecutedCompareResultsEffect(diceModification);
                break;
            default:
                break;
        }

        diceModification.ActionEffect(delegate { ReGenerateListOfButtons(diceModification.DiceModificationTiming); });
    }

    private static void ReGenerateListOfButtons(DiceModificationTimingType timingType)
    {
        HideDiceModificationButtons();

        switch (timingType)
        {
            case DiceModificationTimingType.Normal:
                ShowDiceModificationButtons();
                break;
            case DiceModificationTimingType.Opposite:
                ShowOppositeDiceModificationButtons(true);
                break;
            case DiceModificationTimingType.CompareResults:
                ShowCompareResultsButtons(true);
                break;
            default:
                break;
        }
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
