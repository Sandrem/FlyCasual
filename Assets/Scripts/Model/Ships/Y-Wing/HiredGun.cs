using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YWing
    {
        public class HiredGun : YWing
        {
            public HiredGun() : base()
            {
                PilotName = "Hired Gun";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/Y-wing/hired-gun.png";
                PilotSkill = 4;
                Cost = 20;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.SalvagedAstromech);

                faction = Faction.Scum;

                SkinName = "Gray";
            }
        }
    }
}
