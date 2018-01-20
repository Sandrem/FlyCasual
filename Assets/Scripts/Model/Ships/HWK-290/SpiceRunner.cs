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
                PilotSkill = 1;
                Cost = 16;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                faction = Faction.Scum;
            }
        }
    }
}
