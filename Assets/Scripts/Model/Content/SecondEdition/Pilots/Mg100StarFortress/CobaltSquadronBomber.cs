using Ship;
using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.Mg100StarFortress
    {
        public class CobaltSquadronBomber : Mg100StarFortress
        {
            public CobaltSquadronBomber() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Cobalt Squadron Bomber",
                    1,
                    63 //,
                    //seImageNumber: 19
                );

                ModelInfo.SkinName = "Cobalt";

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/6b6a3bb8049699e2d66fe09531e8bc00.png";
            }
        }
    }
}