using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace M3AScyk
    {
        public class TansariiPointVeteran : M3AScyk
        {
            public TansariiPointVeteran() : base()
            {
                PilotName = "Tansarii Point Veteran";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/M3-A%20Interceptor/tansarii-point-veteran.png";
                PilotSkill = 5;
                Cost = 17;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Serissu";
            }
        }
    }
}
