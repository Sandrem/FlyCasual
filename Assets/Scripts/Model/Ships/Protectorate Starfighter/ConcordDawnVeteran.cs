using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace ProtectorateStarfighter
    {
        public class ConcordDawnVeteran : ProtectorateStarfighter
        {
            public ConcordDawnVeteran() : base()
            {
                PilotName = "Concord Dawn Veteran";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/Protectorate%20Starfighter/concord-dawn-veteran.png";
                PilotSkill = 3;
                Cost = 22;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
