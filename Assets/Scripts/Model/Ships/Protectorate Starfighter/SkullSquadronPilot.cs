using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace ProtectorateStarfighter
    {
        public class SkullSquadronPilot : ProtectorateStarfighter, ISecondEditionPilot
        {
            public SkullSquadronPilot() : base()
            {
                PilotName = "Skull Squadron Pilot";
                PilotSkill = 4;
                Cost = 50;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                SEImageNumber = 159;
            }

            public void AdaptPilotToSecondEdition()
            {
                // No adaptation is needed
            }
        }
    }
}
