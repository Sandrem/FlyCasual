using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class SwarmTactics : GenericUpgrade
    {

        public SwarmTactics() : base()
        {
            Type = UpgradeSlot.Elite;
            Name = ShortName = "Swarm Tactics";
            ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/7/75/Swarm_Tactics.png";
            Cost = 2;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            Phases.OnCombatPhaseStart += SwarmTacticsPilotAbility;
        }

        private void SwarmTacticsPilotAbility()
        {
            //Show Wondow To Select Ally
            // if ally
            // if distance 1-2
            // then increase pilotskill
            // subscribe to pilot skill redution to previous
        }

    }

}
