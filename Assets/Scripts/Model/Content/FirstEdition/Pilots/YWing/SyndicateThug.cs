using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.YWing
    {
        public class SyndicateThug : YWing
        {
            public SyndicateThug() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Syndicate Thug",
                    2,
                    18
                );

                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Astromech);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.SalvagedAstromech);

                ShipInfo.Faction = Faction.Scum;

                ModelInfo.SkinName = "Brown";
            }
        }
    }
}
