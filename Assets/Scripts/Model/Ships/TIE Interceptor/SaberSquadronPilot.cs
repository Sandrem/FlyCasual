using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class SaberSquadronPilot : TIEInterceptor
        {
            public SaberSquadronPilot() : base()
            {
                PilotName = "Saber Squadron Pilot";
                ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/5/5f/Saber_Squadron_Pilot.png";
                PilotSkill = 4;
                Cost = 21;

                AddUpgradeSlot(Upgrade.UpgradeSlot.Elite);
            }
        }
    }
}
