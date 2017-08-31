using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEDefender
    {
        public class GlaiveSquadronPilot : TIEDefender
        {
            public GlaiveSquadronPilot() : base()
            {
                PilotName = "Glaive Squadron Pilot";
                ImageUrl = "https://vignette4.wikia.nocookie.net/xwing-miniatures/images/5/59/Swx52-glaive-squadron-pilot.png";
                PilotSkill = 6;
                Cost = 34;

                AddUpgradeSlot(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
