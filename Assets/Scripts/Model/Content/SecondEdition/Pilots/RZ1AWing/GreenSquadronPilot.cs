using System.Collections;
using System.Collections.Generic;

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
                    34
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                ModelInfo.SkinName = "Green";

                SEImageNumber = 21;
            }
        }
    }
}