using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class GreenSquadronPilot : RZ1AWing
        {
            public GreenSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Green Squadron Pilot",
                    3,
                    34,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 21
                );
                
                ModelInfo.SkinName = "Green";
            }
        }
    }
}