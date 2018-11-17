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
                    "Gold Squadron Pilot",
                    4,
                    20
                );

                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Astromech);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.SalvagedAstromech);

                ShipInfo.Faction = Faction.Scum;

                ModelInfo.SkinName = "Gray";
            }
        }
    }
}
