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
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/Z-95%20Headhunter/binayre-pirate.png";
                PilotSkill = 1;
                Cost = 12;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                SkinName = "Red";

                faction = Faction.Scum;
            }
        }
    }
}
