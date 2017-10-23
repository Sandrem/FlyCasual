using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace AlphaClassStarWing
    {
        public class RhoSquadronVeteran : AlphaClassStarWing
        {
            public RhoSquadronVeteran() : base()
            {
                PilotName = "Rho Squadron Veteran";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/Alpha-class%20Star%20Wing/rho-squadron-veteran.png";
                PilotSkill = 4;
                Cost = 21;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
