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
            UI.ShowSkipButton();
        }

        public override void PerformFreeAction()
        {
            (Phases.CurrentSubPhase as SubPhases.FreeActionDecisonSubPhase).ShowActionDecisionPanel();
            UI.ShowSkipButton();
        }

        public override void PerformAttack()
        {
            UI.ShowSkipButton();
        }

        public override void UseOwnDiceModifications()
        {
            Combat.ShowOwnDiceResultMenu();
        }

        public override void UseOppositeDiceModifications()
        {
            Combat.ShowOppositeDiceResultMenu();
        }

        public override void TakeDecision()
        {
            GameObject.Find("UI").transform.Find("DecisionsPanel").gameObject.SetActive(true);
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
                Messages.ShowError("Attack is not legal (this ship cannot attack or target cannot be attacked)");
            }
            else if (!Combat.ShotInfo.InShotAngle)
            {
                Messages.ShowError("Target is outside your firing arc");
            }
            else if (Combat.ShotInfo.Range > Combat.ChosenWeapon.MaxRange || Combat.ShotInfo.Distance < Combat.ChosenWeapon.MinRange)
            {
                Messages.ShowError("Target is outside your firing range");
            }

            //TODO: except non-legal targets, bupmed for example, biggs?
            Roster.HighlightShipsFiltered(Roster.AnotherPlayer(Phases.CurrentPhasePlayer));
            UI.HighlightNextButton();
        }

    }

}
