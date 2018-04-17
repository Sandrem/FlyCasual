using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{

    public partial class NetworkOpponentPlayer : GenericPlayer
    {

        public NetworkOpponentPlayer() : base()
        {
            Type = PlayerType.Network;
            Name = "Network";
        }

        public override void AfterShipMovementPrediction()
        {
            Selection.ThisShip.AssignedManeuver.LaunchShipMovement();
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

        public override void ConfirmDiceCheck()
        {
            (Phases.CurrentSubPhase as SubPhases.DiceRollCheckSubPhase).ShowConfirmButton();
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

    }

}
