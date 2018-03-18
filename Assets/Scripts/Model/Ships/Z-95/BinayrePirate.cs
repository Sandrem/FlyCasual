using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Z95
    {
        public class BinayrePirate : Z95
        {
            public BinayrePirate() : base()
            {
                PilotName = "Binayre Pirate";
                PilotSkill = 1;
                Cost = 12;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                faction = Faction.Scum;
            }
        }
    }
}
