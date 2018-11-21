using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.AWing
    {
        public class GreenSquadronPilot : AWing
        {
            public GreenSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Green Squadron Pilot",
                    3,
                    19
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ModelInfo.SkinName = "Green";
            }
        }
    }
}