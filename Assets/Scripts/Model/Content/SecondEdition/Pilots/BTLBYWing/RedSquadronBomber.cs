using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLBYWing
    {
        public class RedSquadronBomber : BTLBYWing
        {
            public RedSquadronBomber() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Red Squadron Bomber",
                    2,
                    31
                );

                ImageUrl = "https://i.imgur.com/aHiK4Fy.png";
            }
        }
    }
}
