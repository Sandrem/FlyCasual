using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace HWK290
    {
        public class SpiceRunner : HWK290
        {
            public SpiceRunner() : base()
            {
                PilotName = "Spice Runner";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/HWK-290/spice-runner.png";
                PilotSkill = 1;
                Cost = 16;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                faction = Faction.Scum;
            }
        }
    }
}
