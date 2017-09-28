using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class TurrPhennir : TIEInterceptor
        {
            public TurrPhennir() : base()
            {
                PilotName = "Turr Phennir";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/TIE%20Interceptor/turr-phennir.png";
                PilotSkill = 7;
                Cost = 25;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Red Stripes";
            }
        }
    }
}
