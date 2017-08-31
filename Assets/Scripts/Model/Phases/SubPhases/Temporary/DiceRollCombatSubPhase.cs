using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SubPhases
{

    public class DiceRollCombatSubPhase : GenericSubPhase
    {
        protected DiceKind dicesType;
        protected int dicesCount;

        protected DiceRoll CurentDiceRoll;
        protected DelegateDiceroll checkResults;

        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            IsTemporary = true;
            CallBack = FinishAction;

            Prepare();
            Initialize();

            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            Game.PrefabsList.CombatDiceResultsMenu.SetActive(true);

            if (Combat.AttackStep == CombatStep.Attack)
            {
                Combat.ShowAttackAnimationAndSound();
            }

            DiceRoll DiceRollCheck;
            DiceRollCheck = new DiceRoll(dicesType, dicesCount, DiceRollCheckType.Combat);
            DiceRollCheck.Roll(checkResults);
        }

        public void ToggleConfirmDiceResultsButton(bool isActive)
        {
            if (isActive)
            {
                if (Roster.GetPlayer(Selection.ActiveShip.Owner.PlayerNo).GetType() == typeof(Players.HumanPlayer))
                {
                    Button closeButton = Game.PrefabsList.CombatDiceResultsMenu.transform.Find("DiceModificationsPanel/Confirm").GetComponent<Button>();
                    closeButton.onClick.RemoveAllListeners();
                    closeButton.onClick.AddListener(delegate { CallBack(); });

                    closeButton.gameObject.SetActive(true);
                }
            }
            else
            {
                Game.PrefabsList.CombatDiceResultsMenu.transform.Find("DiceModificationsPanel/Confirm").gameObject.SetActive(false);
            }
        }

        protected virtual void CheckResults(DiceRoll diceRoll)
        {
            CurentDiceRoll = diceRoll;
            Combat.ToggleConfirmDiceResultsButton(true);
            Combat.ShowDiceModificationButtons();
        }

        protected virtual void FinishAction()
        {
            HideDiceResultMenu();
            Phases.FinishSubPhase(this.GetType());
        }

        public void HideDiceResultMenu()
        {
            Game.PrefabsList.CombatDiceResultsMenu.SetActive(false);
            HideDiceModificationButtons();
            CurentDiceRoll.RemoveDiceModels();
        }

        public void HideDiceModificationButtons()
        {
            foreach (Transform button in Game.PrefabsList.CombatDiceResultsMenu.transform.Find("DiceModificationsPanel"))
            {
                if (button.name.StartsWith("Button"))
                {
                    MonoBehaviour.Destroy(button.gameObject);
                }
            }
            ToggleConfirmDiceResultsButton(false);
        }

        public override void Pause()
        {
            Game.PrefabsList.CombatDiceResultsMenu.SetActive(false);
        }

        public override void Resume()
        {
            Game.PrefabsList.CombatDiceResultsMenu.SetActive(true);
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
