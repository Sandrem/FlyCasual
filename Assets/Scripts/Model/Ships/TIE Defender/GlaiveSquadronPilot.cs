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
                PilotSkill = 6;
                Cost = 34;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Crimson";
            }
        }
    }
}
