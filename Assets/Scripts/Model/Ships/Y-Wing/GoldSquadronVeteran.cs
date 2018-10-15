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
                Cost = 34;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                faction = Faction.Rebel;

                SEImageNumber = 17;
            }

            public void AdaptPilotToSecondEdition()
            {
                // No Changes
            }
        }
    }
}
