using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace ScurrgH6Bomber
    {
        public class LokRevenant : ScurrgH6Bomber
        {
            public LokRevenant() : base()
            {
                PilotName = "Lok Revenant";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/Scurrg%20H-6%20Bomber/lok-revenant.png";
                PilotSkill = 3;
                Cost = 26;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
