using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Hwk290LightFreighter
    {
        public class SpiceRunner : Hwk290LightFreighter
        {
            public SpiceRunner() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Spice Runner",
                    1,
                    32
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

                ShipInfo.Faction = Faction.Scum;

                SEImageNumber = 177;
            }
        }
    }
}
