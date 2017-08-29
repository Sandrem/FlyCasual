using System;
using System.Collections;
using System.Collections.Generic;
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
            Game.UI.ShowSkipButton();
        }

        public override void PerformFreeAction()
        {
            (Phases.CurrentSubPhase as SubPhases.FreeActionDecisonSubPhase).ShowActionDecisionPanel();
            Game.UI.ShowSkipButton();
        }

        public override void PerformAttack()
        {
            Game.UI.ShowSkipButton();
        }

        public override void UseDiceModifications()
        {
            Combat.ShowDiceResultMenu(Combat.ConfirmDiceResults);
        }

        public override void TakeDecision()
        {
            Game.PrefabsList.PanelDecisions.SetActive(true);
        }

        public override void AfterShipMovementPrediction()
        {
            Selection.ThisShip.AssignedManeuver.LaunchShipMovement();
        }

    }

}
