using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace ProtectorateStarfighter
    {
        public class ConcordDawnAce : ProtectorateStarfighter
        {
            public ConcordDawnAce() : base()
            {
                PilotName = "Concord Dawn Ace";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/Protectorate%20Starfighter/concord-dawn-ace.png";
                PilotSkill = 5;
                Cost = 24;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
