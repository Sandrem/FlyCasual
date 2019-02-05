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
                    32,
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Talent, UpgradeType.Talent },
                    seImageNumber: 21
                );
                
                ModelInfo.SkinName = "Green";
            }
        }
    }
}