using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.ScurrgH6Bomber
    {
        public class LokRevenant : ScurrgH6Bomber
        {
            public LokRevenant() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lok Revenant",
                    3,
                    26
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
            }
        }
    }
}
