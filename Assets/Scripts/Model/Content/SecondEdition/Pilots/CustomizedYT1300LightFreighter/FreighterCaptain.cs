using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.CustomizedYT1300LightFreighter
    {
        public class FreighterCaptain : CustomizedYT1300LightFreighter
        {
            public FreighterCaptain() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Freighter Captain",
                    1,
                    46,
                    seImageNumber: 225
                );
            }
        }
    }
}
