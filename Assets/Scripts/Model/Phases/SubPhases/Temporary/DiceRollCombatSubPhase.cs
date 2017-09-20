using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SubPhases
{

    public class DiceRollCombatSubPhase : GenericSubPhase
    {
        protected DiceKind diceType;
        protected int diceCount;

        protected DiceRoll CurentDiceRoll;
        protected DelegateDiceroll checkResults;

        public override void Start()
        {
            IsTemporary = true;
            CallBack = FinishAction;

            Prepare();
            Initialize();

            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").gameObject.SetActive(true);

            if (Combat.AttackStep == CombatStep.Attack)
            {
                Combat.ShowAttackAnimationAndSound();
            }

            DiceRoll DiceRollCheck;
            DiceRollCheck = new DiceRoll(diceType, diceCount, DiceRollCheckType.Combat);
            DiceRollCheck.Roll(ImmediatelyAfterRolling);
        }

        private void ImmediatelyAfterRolling(DiceRoll diceroll)
        {
            Selection.ActiveShip = (Combat.AttackStep == CombatStep.Attack) ? Combat.Attacker : Combat.Defender;
            Selection.ActiveShip.CallOnImmediatelyAfterRolling(diceroll);
            checkResults(diceroll);
        }

        public void ToggleConfirmDiceResultsButton(bool isActive)
        {
            if (isActive)
            {
                if (Roster.GetPlayer(Selection.ActiveShip.Owner.PlayerNo).GetType() == typeof(Players.HumanPlayer))
                {
                    Button closeButton = GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel/Confirm").GetComponent<Button>();
                    closeButton.onClick.RemoveAllListeners();
                    closeButton.onClick.AddListener(delegate { CallBack(); });

                    closeButton.gameObject.SetActive(true);
                }
            }
            else
            {
                GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").Find("DiceModificationsPanel").Find("Confirm").gameObject.SetActive(false);
            }
        }

        protected virtual void CheckResults(DiceRoll diceRoll)
        {
            CurentDiceRoll = diceRoll;
            Selection.ActiveShip = (Combat.AttackStep == CombatStep.Attack) ? Combat.Defender : Combat.Attacker;
            Selection.ActiveShip.Owner.UseOppositeDiceModifications();
        }

        protected virtual void FinishAction()
        {
            HideDiceResultMenu();
            Phases.FinishSubPhase(this.GetType());
        }

        public void HideDiceResultMenu()
        {
            GameObject.Find("UI/CombatDiceResultsPanel").gameObject.SetActive(false);
            HideDiceModificationButtons();
            CurentDiceRoll.RemoveDiceModels();
        }

        public void HideDiceModificationButtons()
        {
            foreach (Transform button in GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel"))
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
            GameObject.Find("UI/CombatDiceResultsPanel").gameObject.SetActive(false);
        }

        public override void Resume()
        {
            GameObject.Find("UI/CombatDiceResultsPanel").gameObject.SetActive(true);
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
