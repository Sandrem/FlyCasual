using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEBomber
    {
        public class GammaSquadronPilot : TIEBomber
        {
            public GammaSquadronPilot() : base()
            {
                PilotName = "Gamma Squadron Pilot";
                PilotSkill = 4;
                Cost = 18;

                SkinName = "White Stripes";
            }
        }
    }
}
