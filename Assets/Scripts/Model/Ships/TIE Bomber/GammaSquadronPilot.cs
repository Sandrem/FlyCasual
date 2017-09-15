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
                ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/d/d0/Gamma_Squadron_Pilot.png";
                PilotSkill = 4;
                Cost = 18;

                nameOfSkin = "White Stripes";
            }
        }
    }
}
