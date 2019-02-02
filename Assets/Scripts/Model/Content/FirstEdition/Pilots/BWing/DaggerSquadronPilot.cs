using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace FirstEdition.BWing
    {
        public class DaggerSquadronPilot : BWing
        {
            public DaggerSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Dagger Squadron Pilot",
                    4,
                    24
                );

                ModelInfo.SkinName = "Red";
            }
        }
    }
}
