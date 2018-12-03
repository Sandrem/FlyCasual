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

                ImageUrl = "http://www.infinitearenas.com/xw2browse/images/resistance/cobalt-squadron-bomber.jpg";
            }
        }
    }
}