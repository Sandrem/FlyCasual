using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YWing
    {
        public class GoldSquadronPilot : YWing
        {
            public GoldSquadronPilot() : base()
            {
                PilotName = "Gold Squadron Pilot";
                PilotSkill = 2;
                Cost = 18;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Astromech);

                faction = Faction.Rebel;
            }
        }
    }
}
