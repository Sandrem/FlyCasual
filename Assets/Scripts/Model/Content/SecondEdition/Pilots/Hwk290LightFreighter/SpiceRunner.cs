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
                    32,
                    extraUpgradeIcon: UpgradeType.Illicit,
                    factionOverride: Faction.Scum,
                    seImageNumber: 177
                );
            }
        }
    }
}
