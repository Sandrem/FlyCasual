using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace AWing
    {
        public class GreenSquadronPilot : AWing
        {
            public GreenSquadronPilot() : base()
            {
                PilotName = "Green Squadron Pilot";
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/2/23/Green_Squadron_Pilot.png";
                PilotSkill = 3;
                Cost = 19;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                nameOfSkin = "Green";
            }
        }
    }
}
