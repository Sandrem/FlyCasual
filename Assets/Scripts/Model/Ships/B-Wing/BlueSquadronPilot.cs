﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace BWing
    {
        public class BlueSquadronPilot : BWing
        {
            public BlueSquadronPilot() : base()
            {
                PilotName = "Blue Squadron Pilot";
                PilotSkill = 2;
                Cost = 22;

                SkinName = "Blue";
            }
        }
    }
}
