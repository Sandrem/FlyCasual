using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEPunisher
    {
        public class CutlassSquadronPilot : TIEPunisher
        {
            public CutlassSquadronPilot() : base()
            {
                PilotName = "Cutlass Squadron Pilot";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/TIE%20Punisher/cutlass-squadron-pilot.png";
                PilotSkill = 2;
                Cost = 21;
            }
        }
    }
}
