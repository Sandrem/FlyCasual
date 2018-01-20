﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace G1AStarfighter
    {
        public class GandFindsman : G1AStarfighter
        {
            public GandFindsman() : base()
            {
                PilotName = "Gand Findsman";
                PilotSkill = 5;
                Cost = 25;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}
