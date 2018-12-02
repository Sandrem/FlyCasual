using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class BlueSquadronRookie : T70XWing
        {
            public BlueSquadronRookie() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Blue Squadron Rookie",
                    1,
                    46 //,
                    //seImageNumber: 11
                );

                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/2/24/Swz25_blue-sqd_a1.png";
            }
        }
    }
}
