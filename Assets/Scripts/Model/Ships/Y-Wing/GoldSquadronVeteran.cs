using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YWing
    {
        public class GoldSquadronVeteran : YWing, ISecondEditionPilot
        {
            public GoldSquadronVeteran() : base()
            {
                PilotName = "Gold Squadron Veteran";
                PilotSkill = 3;
                Cost = 37;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Astromech);

                PilotRuleType = typeof(SecondEdition);

                faction = Faction.Rebel;
            }

            public void AdaptPilotToSecondEdition()
            {
                // No Changes
            }
        }
    }
}
