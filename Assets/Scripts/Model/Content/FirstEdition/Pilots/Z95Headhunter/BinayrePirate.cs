using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.Z95Headhunter
    {
        public class BinayrePirate : Z95Headhunter
        {
            public BinayrePirate() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Binayre Pirate",
                    1,
                    12
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

                ShipInfo.Faction = Faction.Scum;
            }
        }
    }
}
