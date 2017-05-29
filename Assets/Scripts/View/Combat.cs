using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Todo: Move to different scripts by menu names

public delegate void DiceModification();

public static partial class Combat
{

    public static void ShowDiceResultMenu()
    {
        Game.PrefabsList.DiceResultsMenu.SetActive(true);
    }

    public static void ShowDiceModificationButtons()
    {
        Selection.ActiveShip.GenerateDiceModificationButtons();

        if (Roster.GetPlayer(Selection.ActiveShip.Owner.PlayerNo).Type == Players.PlayerType.Human)
        {
            float offset = 0;
            Vector3 defaultPosition = Game.PrefabsList.DiceResultsMenu.transform.position + new Vector3(5, 195, 0);

            foreach (var actionEffect in Selection.ActiveShip.AvailableActionEffects)
            {
                Vector3 position = defaultPosition + new Vector3(0, -offset, 0);
                CreateDiceModificationsButton(actionEffect, position);
                offset += 40;
            }

            //TODO: Fix size of ActionEffect Menu

            Game.PrefabsList.DiceResultsMenu.transform.Find("Confirm").gameObject.SetActive(true);
        }

        Roster.GetPlayer(Selection.ActiveShip.Owner.PlayerNo).UseDiceModifications();
    }

    private static void CreateDiceModificationsButton(ActionsList.GenericAction actionEffect, Vector3 position)
    {
        GameObject newButton = MonoBehaviour.Instantiate(Game.PrefabsList.GenericButton, Game.PrefabsList.DiceResultsMenu.transform);
        newButton.name = "Button" + actionEffect.EffectName;
        newButton.transform.GetComponentInChildren<Text>().text = actionEffect.EffectName;
        newButton.GetComponent<RectTransform>().position = position;
        newButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            actionEffect.ActionEffect();
            Tooltips.EndTooltip();
            newButton.GetComponent<Button>().interactable = false;
        });
        Tooltips.AddTooltip(newButton, actionEffect.ImageUrl);
        newButton.GetComponent<Button>().interactable = true;
        newButton.SetActive(true);
    }

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

    public static void ConfirmDiceResults()
    {
        HideDiceResultMenu();

        if (AttackStep == CombatStep.Attack)
        {
            PerformDefence(Selection.ThisShip, Selection.AnotherShip);
        }
        else if ((AttackStep == CombatStep.Defence))
        {
            //TODO: Show compare results dialog
            CalculateAttackResults(Selection.ThisShip, Selection.AnotherShip);

            MovementTemplates.ReturnRangeRuler();

            if (Roster.NoSamePlayerAndPilotSkillNotAttacked(Selection.ThisShip))
            {
                Phases.CurrentSubPhase.NextSubPhase();
            }

        }
    }

    private static void HideDiceResultMenu()
    {
        Game.PrefabsList.DiceResultsMenu.SetActive(false);
        HideDiceModificationButtons();
        CurentDiceRoll.RemoveDiceModels();
    }

}
