using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace HWK290
    {
        public class SpiceRunner : HWK290, ISecondEditionPilot
        {
            public SpiceRunner() : base()
            {
                PilotName = "Spice Runner";
                PilotSkill = 1;
                Cost = 16;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                faction = Faction.Scum;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 1;
                Cost = 32;
            }
        }
    }
}
