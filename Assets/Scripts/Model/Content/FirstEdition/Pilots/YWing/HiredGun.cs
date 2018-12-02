using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.YWing
    {
        public class HiredGun : YWing
        {
            public HiredGun() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hired Gun",
                    4,
                    20,
                    factionOverride: Faction.Scum
                );

                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Astromech);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.SalvagedAstromech);

                ModelInfo.SkinName = "Gray";
            }
        }
    }
}
