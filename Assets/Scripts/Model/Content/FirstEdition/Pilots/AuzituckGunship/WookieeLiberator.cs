using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.AuzituckGunship
    {
        public class WookieeLiberator : AuzituckGunship
        {
            public WookieeLiberator() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Wookiee Liberator",
                    3,
                    26
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
            }
        }
    }
}