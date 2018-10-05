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

        public override void ConfirmDiceCheck()
        {
            (Phases.CurrentSubPhase as SubPhases.DiceRollCheckSubPhase).ShowConfirmButton();
        }

        public override void PerformTractorBeamReposition(GenericShip ship)
        {
            RulesList.TractorBeamRule.PerfromManualTractorBeamReposition(ship, this);
        }

        public override void InformAboutCrit()
        {
            base.InformAboutCrit();

            InformCrit.ShowConfirmButton();
        }
    }

}
