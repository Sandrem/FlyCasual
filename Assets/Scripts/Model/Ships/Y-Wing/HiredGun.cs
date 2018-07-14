using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YWing
    {
        public class HiredGun : YWing, ISecondEditionPilot
        {
            public HiredGun() : base()
            {
                PilotName = "Hired Gun";
                PilotSkill = 4;
                Cost = 20;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.SalvagedAstromech);

                faction = Faction.Scum;

                SkinName = "Gray";
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
            }
        }
    }
}
