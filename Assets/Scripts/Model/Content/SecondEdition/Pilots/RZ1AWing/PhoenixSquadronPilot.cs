using System.Collections;
using System.Collections.Generic;

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
                    30
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                ModelInfo.SkinName = "Blue";

                SEImageNumber = 22;
            }
        }
    }
}