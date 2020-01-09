using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.ModifiedYT1300LightFreighter
    {
        public class OuterRimSmuggler : ModifiedYT1300LightFreighter
        {
            public OuterRimSmuggler() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Outer Rim Smuggler",
                    1,
                    67,
                    seImageNumber: 72
                );
            }
        }
    }
}
