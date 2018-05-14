using RuleSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace ProtectorateStarfighter
    {
        public class ZealousRecruit : ProtectorateStarfighter, ISecondEditionPilot
        {
            public ZealousRecruit() : base()
            {
                PilotName = "Zealous Recruit";
                PilotSkill = 1;
                Cost = 20;
            }

            public void AdaptPilotToSecondEdition()
            {
                ImageUrl = "https://i.imgur.com/3NyVv05.png";
            }
        }
    }
}
