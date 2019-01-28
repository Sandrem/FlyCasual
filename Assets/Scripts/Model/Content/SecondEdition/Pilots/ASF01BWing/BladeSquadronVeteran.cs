using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ASF01BWing
    {
        public class BladeSquadronVeteran : ASF01BWing
        {
            public BladeSquadronVeteran() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Blade Squadron Veteran",
                    3,
                    43,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 25
                );

                ModelInfo.SkinName = "Blue";
            }
        }
    }
}
