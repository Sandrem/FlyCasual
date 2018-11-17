using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class HiredGun : BTLA4YWing
        {
            public HiredGun() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hired Gun",
                    2,
                    34
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

                ShipInfo.Faction = Faction.Scum;

                ModelInfo.SkinName = "Gray";

                SEImageNumber = 167;
            }
        }
    }
}
