using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Astromech);

                SkinName = "Gray";

                faction = Faction.Rebel;
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotName = "Gray Squadron Bomber";
                PilotSkill = 2;
                ImageUrl = "https://i.imgur.com/1tN3KEx.png";
            }
        }
    }
}
