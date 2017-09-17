using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEAgressor
    {
        public class OnyxSquadronEscort : TIEAgressor
        {
            public OnyxSquadronEscort() : base()
            {
                PilotName = "Onyx Squadron Escort";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/TIE%20Aggressor/onyx-squadron-escort.png";
                PilotSkill = 5;
                Cost = 19;
            }
        }
    }
}
