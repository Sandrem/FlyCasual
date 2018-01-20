﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace YWing
    {
        public class GraySquadronPilot : YWing
        {
            public GraySquadronPilot() : base()
            {
                PilotName = "Gray Squadron Pilot";
                PilotSkill = 4;
                Cost = 20;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Astromech);

                SkinName = "Gray";

                faction = Faction.Rebel;
            }
        }
    }
}
