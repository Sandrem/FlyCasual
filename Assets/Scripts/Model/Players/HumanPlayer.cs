using System;
using System.Collections;
using System.Collections.Generic;
using Ship;
using UnityEngine;

namespace Players
{

    public partial class HumanPlayer : GenericPlayer
    {

        public HumanPlayer() : base()
        {
            Type = PlayerType.Human;
            Name = "Human";
        }

        public override void PerformAction()
        {
            (Phases.CurrentSubPhase as SubPhases.ActionDecisonSubPhase).ShowActionDecisionPanel();
        }

        public override void PerformFreeAction()
        {
            (Phases.CurrentSubPhase as SubPhases.FreeActionDecisonSubPhase).ShowActionDecisionPanel();
        }

        public override void PerformAttack()
        {
            base.PerformAttack();

            UI.ShowSkipButton();
        }

        public override void UseOwnDiceModifications()
        {
            base.UseOwnDiceModifications();

            Combat.ShowOwnDiceResultMenu();
        }

        public override void UseOppositeDiceModifications()
        {
            base.UseOppositeDiceModifications();

            Combat.ShowOppositeDiceResultMenu();
        }

        public override void UseCompareResultsDiceModifications()
        {
            base.UseCompareResultsDiceModifications();

            Combat.ShowCompareResultsMenu();
        }

        public override void TakeDecision()
        {
            (Phases.CurrentSubPhase as SubPhases.DecisionSubPhase).ShowDecisionWindowUI();
        }

        public override void AfterShipMovementPrediction()
        {
            Selection.ThisShip.AssignedManeuver.LaunchShipMovement();
        }

        public override void ConfirmDiceCheck()
        {
            (Phases.CurrentSubPhase as SubPhases.DiceRollCheckSubPhase).ShowConfirmButton();
        }

        public override void ToggleCombatDiceResults(bool isActive)
        {
            (Phases.CurrentSubPhase as SubPhases.DiceRollCombatSubPhase).ToggleConfirmButton(isActive);
        }

        public override bool IsNeedToShowManeuver(GenericShip ship)
        {
            return true;
        }

        public override void OnTargetNotLegalForAttack()
        {
            // TODO: Better explanations
            if (!Rules.TargetIsLegalForShot.IsLegal())
            {
                //automatic error messages
            }
            else if (!Combat.ShotInfo.InShotAngle)
            {
                Messages.ShowErrorToHuman("Target is outside your firing arc");
            }
            else if (Combat.ShotInfo.Range > Combat.ChosenWeapon.MaxRange || Combat.ShotInfo.Distance < Combat.ChosenWeapon.MinRange)
            {
                Messages.ShowErrorToHuman("Target is outside your firing range");
            }

            //TODO: except non-legal targets, bupmed for example, biggs?
            Roster.HighlightShipsFiltered(FilterShipsToAttack);

            UI.ShowSkipButton();
            UI.HighlightNextButton();

            if (Phases.CurrentSubPhase is SubPhases.ExtraAttackSubPhase)
            {
                (Phases.CurrentSubPhase as SubPhases.ExtraAttackSubPhase).RevertSubphase();
            }
        }

        private bool FilterShipsToAttack(GenericShip ship)
        {
            return ship.Owner.PlayerNo != Phases.CurrentSubPhase.RequiredPlayer;
        }

        public override void ChangeManeuver(Action<string> callback, Func<string, bool> filter = null)
        {
            DirectionsMenu.Show(callback, filter);
        }

        public override void SelectManeuver(Action<string> callback, Func<string, bool> filter = null)
        {
            DirectionsMenu.Show(callback, filter);
        }

        public override void SelectShipForAbility()
        {
            GameModes.GameMode.CurrentGameMode.StartSyncSelectShipPreparation();
        }

        public override void RerollManagerIsPrepared()
        {
            DiceRerollManager.CurrentDiceRerollManager.ShowConfirmButton();
        }
    }

}
