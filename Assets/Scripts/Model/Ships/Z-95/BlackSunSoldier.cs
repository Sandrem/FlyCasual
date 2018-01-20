using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Z95
    {
        public class BlackSunSoldier : Z95
        {
            public BlackSunSoldier() : base()
            {
                PilotName = "Black Sun Soldier";
                PilotSkill = 3;
                Cost = 13;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                SkinName = "Black Sun";

                faction = Faction.Scum;
            }
        }
    }
}
