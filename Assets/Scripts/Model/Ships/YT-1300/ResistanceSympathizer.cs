using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YT1300
    {
        public class ResistanceSympathizer : YT1300
        {
            public ResistanceSympathizer() : base()
            {
                PilotName = "Resistance Sympathizer";
                PilotSkill = 3;
                Cost = 38;

 			    Firepower = 3;
                MaxHull = 8;
                MaxShields = 5;

                SubFaction = SubFaction.Resistance;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);

            }
        }
    }
}
