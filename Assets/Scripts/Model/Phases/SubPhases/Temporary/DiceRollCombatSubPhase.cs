using ActionsList;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SubPhases
{

    public class DiceRollCombatSubPhase : GenericSubPhase
    {
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.SyncDiceRerollSelected, GameCommandTypes.SyncDiceResults }; } }

        protected DiceKind diceType;
        protected int diceCount;

        protected DiceRoll CurentDiceRoll;
        protected DelegateDiceroll checkResults;

        public override void Start()
        {
            base.Start();

            IsTemporary = true;
            CallBack = FinishAction;

            Prepare();
            Initialize();

            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            

            if (Combat.AttackStep == CombatStep.Attack)
            {
                Combat.Attacker.CallDiceAboutToBeRolled(RollDice);
            }
            else
            {
                Combat.Defender.CallDiceAboutToBeRolled(RollDice);
            }
        }

        private void RollDice()
        {
            DiceRoll DiceRollCombat;
            DiceRollCombat = new DiceRoll(diceType, diceCount, DiceRollCheckType.Combat, Selection.ActiveShip.Owner.PlayerNo);
            DiceRollCombat.Roll(delegate { ImmediatelyAfterRolling(); });
        }

        private void ImmediatelyAfterRolling()
        {
            Selection.ActiveShip = (Combat.AttackStep == CombatStep.Attack) ? Combat.Attacker : Combat.Defender;
            Selection.ActiveShip.CallOnImmediatelyAfterRolling(DiceRoll.CurrentDiceRoll, delegate { FinallyCheckResults(DiceRoll.CurrentDiceRoll); });
        }

        private void FinallyCheckResults(DiceRoll diceroll)
        {
            checkResults(diceroll);
        }

        public void PrepareToggleConfirmButton(bool isActive)
        {
            Roster.GetPlayer(Selection.ActiveShip.Owner.PlayerNo).ToggleCombatDiceResults(isActive);
        }

        protected virtual void CheckResults(DiceRoll diceRoll)
        {
            CurentDiceRoll = diceRoll;
            Selection.ActiveShip = (Combat.AttackStep == CombatStep.Attack) ? Combat.Defender : Combat.Attacker;

            Combat.DiceModifications.Next();
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
            PrepareToggleConfirmButton(false);
        }

        public override void Pause()
        {
            GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").gameObject.SetActive(false);
        }

        public override void Resume()
        {
            base.Resume();

            GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").gameObject.SetActive(true);
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

        public void ToggleConfirmButton(bool isActive)
        {
            Button closeButton = GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").Find("DiceModificationsPanel").Find("Confirm").GetComponent<Button>();
            if (isActive)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(delegate { CallBack(); });
            }
            closeButton.gameObject.SetActive(isActive);
        }

    }

    public class AttackDiceRollCombatSubPhase : DiceRollCombatSubPhase
    {
        public override void Prepare()
        {
            CanBePaused = true;

            diceType = DiceKind.Attack;
            diceCount = Combat.Attacker.GetNumberOfAttackDice(Combat.Defender);

            checkResults = CheckResults;
        }

        protected override void CheckResults(DiceRoll diceRoll)
        {
            Selection.ActiveShip = Selection.ThisShip;

            Combat.CurrentDiceRoll = diceRoll;
            Combat.DiceRollAttack = diceRoll;

            Combat.Attacker.CallCheckCancelCritsFirst();
            Combat.Defender.CallCheckCancelCritsFirst();

            base.CheckResults(diceRoll);
        }

        public override void Pause()
        {
            GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").gameObject.SetActive(false);
        }

        public override void Resume()
        {
            GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").gameObject.SetActive(true);
        }

        public override void Next()
        {
            CurentDiceRoll.RemoveDiceModels();
            Phases.CurrentSubPhase = PreviousSubPhase;

            Combat.DiceModifications.Next();
        }
    }

    public class DefenseDiceRollCombatSubPhase : DiceRollCombatSubPhase
    {
        public override void Prepare()
        {
            diceType = DiceKind.Defence;
            diceCount = Combat.Defender.GetNumberOfDefenceDice(Combat.Attacker);

            checkResults = CheckResults;

            new DiceCompareHelper(Combat.DiceRollAttack);
        }

        protected override void CheckResults(DiceRoll diceRoll)
        {
            Selection.ActiveShip = Selection.AnotherShip;

            Combat.CurrentDiceRoll = diceRoll;
            Combat.DiceRollDefence = diceRoll;

            base.CheckResults(diceRoll);
        }

        public override void Next()
        {
            CurentDiceRoll.RemoveDiceModels();
            Phases.CurrentSubPhase = PreviousSubPhase;

            Combat.DiceModifications.Next();
        }

    }

}
