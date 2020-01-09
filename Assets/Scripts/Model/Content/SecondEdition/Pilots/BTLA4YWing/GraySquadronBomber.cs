﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class GraySquadronBomber : BTLA4YWing
        {
            public GraySquadronBomber() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Gray Squadron Bomber",
                    2,
                    30,
                    seImageNumber: 18
                );
            }
        }
    }
}
