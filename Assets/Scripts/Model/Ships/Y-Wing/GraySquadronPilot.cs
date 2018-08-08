using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace YWing
    {
        public class GraySquadronPilot : YWing, ISecondEditionPilot
        {
            public GraySquadronPilot() : base()
            {
                PilotName = "Gray Squadron Pilot";
                PilotSkill = 4;
                Cost = 20;

                PrintedUpgradeIcons.Add(UpgradeType.Astromech);

                SkinName = "Gray";

                faction = Faction.Rebel;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotName = "Gray Squadron Bomber";
                PilotNameShort = "Gray Sq. Bomber";
                PilotSkill = 2;

                Cost = 32;
            }
        }
    }
}
