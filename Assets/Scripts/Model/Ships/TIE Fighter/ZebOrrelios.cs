using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFighter
    {
        public class ZebOrrelios : TIEFighter
        {
            public ZebOrrelios() : base()
            {
                PilotName = "Generic Pilot";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Rebel%20Alliance/TIE%20Fighter/zeb-orrelios.png";
                PilotSkill = 3;
                Cost = 13;

                faction = Faction.Rebels;
            }
        }
    }
}
