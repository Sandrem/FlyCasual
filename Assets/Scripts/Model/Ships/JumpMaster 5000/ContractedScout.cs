using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace JumpMaster5000
    {
        public class ContractedScout : JumpMaster5000
        {
            public ContractedScout() : base()
            {
                PilotName = "Contracted Scout";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Scum%20and%20Villainy/JumpMaster%205000/contracted-scout.png";
                PilotSkill = 3;
                Cost = 25;
            }
        }
    }
}
