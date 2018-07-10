using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace AlphaClassStarWing
    {
        public class RhoSquadronPilot : AlphaClassStarWing, ISecondEditionPilot
        {
            public RhoSquadronPilot() : base()
            {
                PilotName = "Rho Squadron Pilot";
                PilotSkill = 4;
                Cost = 21;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
            }
        }
    }
}
