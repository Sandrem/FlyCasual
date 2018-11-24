using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class PhoenixSquadronPilot : RZ1AWing
        {
            public PhoenixSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Phoenix Squadron Pilot",
                    1,
                    30,
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 22
                );
                
                ModelInfo.SkinName = "Blue";
            }
        }
    }
}