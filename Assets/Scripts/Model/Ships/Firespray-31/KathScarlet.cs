using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Firespray31
    {
        public class KathScarletEmpire : Firespray31
        {
            public KathScarletEmpire() : base()
            {
                PilotName = "Kath Scarlet";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/Firespray-31/kath-scarlet.png";
                PilotSkill = 7;
                Cost = 38;

                SkinName = "Kath Scarlet";

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                faction = Faction.Empire;
            }
        }
    }
}
