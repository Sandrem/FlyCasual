using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Vcx100
    {
        public class LothalRebel : Vcx100
        {
            public LothalRebel() : base()
            {
                PilotName = "Lothal Rebel";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Rebel%20Alliance/VCX-100/lothal-rebel.png";
                PilotSkill = 3;
                Cost = 35;
            }
        }
    }
}
