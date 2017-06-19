using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SubPhases
{

    public class DiceRollCombatSubPhase : GenericSubPhase
    {
        protected string dicesType;
        protected int dicesCount;

        protected DiceRoll CurentDiceRoll;
        protected DelegateDiceroll checkResults;

        protected UnityEngine.Events.UnityAction finishAction;

        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            isTemporary = true;
            finishAction = FinishAction;

            Prepare();
            Initialize();

            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            Game.PrefabsList.DiceResultsMenu.SetActive(true);

            if (Combat.AttackStep == CombatStep.Attack)
            {
                Combat.ShowAttackAnimationAndSound();
            }

            DiceRoll DiceRollCheck;
            DiceRollCheck = new DiceRoll(dicesType, dicesCount);
            DiceRollCheck.Roll();
            DiceRollCheck.CalculateResults(checkResults);
        }

        public void ShowConfirmDiceResultsButton()
        {
            if (Roster.GetPlayer(Selection.ActiveShip.Owner.PlayerNo).GetType() == typeof(Players.HumanPlayer))
            {
                Button closeButton = Game.PrefabsList.DiceResultsMenu.transform.Find("Confirm").GetComponent<Button>();
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(finishAction);

                closeButton.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("AI waits to press Confirm button");
                Game.Wait(finishAction.Invoke);
            }
        }

        protected virtual void CheckResults(DiceRoll diceRoll)
        {
            Debug.Log("Combat Dice Roll - check action");
            CurentDiceRoll = diceRoll;
            Combat.ShowDiceModificationButtons();
            ShowConfirmDiceResultsButton();
        }

        protected virtual void FinishAction()
        {
            HideDiceResultMenu();
            Phases.FinishSubPhase(this.GetType());
        }

        public void HideDiceResultMenu()
        {
            Game.PrefabsList.DiceResultsMenu.SetActive(false);
            HideDiceModificationButtons();
            CurentDiceRoll.RemoveDiceModels();
        }

        public void HideDiceModificationButtons()
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

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip)
        {
            bool result = false;
            return result;
        }

    }

}
