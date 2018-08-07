using ActionsList;
using Ship;
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

        public override void ConfirmDiceCheck()
        {
            (Phases.CurrentSubPhase as SubPhases.DiceRollCheckSubPhase).ShowConfirmButton();
        }

        public override void UseOwnDiceModifications()
        {
            base.UseOwnDiceModifications();

            Combat.ShowDiceModificationButtons(DiceModificationTimingType.Normal);
        }

        public override void UseOppositeDiceModifications()
        {
            base.UseOppositeDiceModifications();

            Combat.ShowDiceModificationButtons(DiceModificationTimingType.Opposite);
        }

        public override void UseCompareResultsDiceModifications()
        {
            base.UseCompareResultsDiceModifications();

            Combat.ShowDiceModificationButtons(DiceModificationTimingType.CompareResults);
        }

        public override void PerformTractorBeamReposition(GenericShip ship)
        {
            RulesList.TractorBeamRule.PerfromManualTractorBeamReposition(ship, this);
        }

    }

}
