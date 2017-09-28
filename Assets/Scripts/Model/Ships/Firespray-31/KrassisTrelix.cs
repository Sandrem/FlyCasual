using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Firespray31
    {
        public class KrassisTrelix : Firespray31
        {
            public KrassisTrelix() : base()
            {
                PilotName = "Krassis Trelix";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/Firespray-31/krassis-trelix.png";
                PilotSkill = 5;
                Cost = 36;

                SkinName = "Krassis Trelix";

                faction = Faction.Empire;
            }
        }
    }
}
