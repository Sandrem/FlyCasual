using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.Hwk290
    {
        public class SpiceRunner : Hwk290
        {
            public SpiceRunner() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Spice Runner",
                    1,
                    16
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

                ShipInfo.Faction = Faction.Scum;
            }
        }
    }
}
