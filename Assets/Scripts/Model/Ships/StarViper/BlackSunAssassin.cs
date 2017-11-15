using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace StarViper
    {
        public class BlackSunAssassin : StarViper
        {
            public BlackSunAssassin() : base()
            {
                PilotName = "Black Sun Assassin";
                PilotSkill = 5;
                Cost = 28;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Black Sun Assassin";
            }
        }
    }
}
