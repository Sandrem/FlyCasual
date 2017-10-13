using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEAdvPrototype
    {
        public class BaronOfTheEmpire : TIEAdvPrototype
        {
            public BaronOfTheEmpire() : base()
            {
                PilotName = "Baron of the Empire";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/TIE%20Adv.%20Prototype/baron-of-the-empire.png";
                PilotSkill = 4;
                Cost = 19;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
