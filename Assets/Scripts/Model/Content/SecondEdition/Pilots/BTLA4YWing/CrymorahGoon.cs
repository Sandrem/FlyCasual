using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class CrymorahGoon : BTLA4YWing
        {
            public CrymorahGoon() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Crymorah Goon",
                    1,
                    32
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

                ShipInfo.Faction = Faction.Scum;

                ModelInfo.SkinName = "Brown";

                SEImageNumber = 168;
            }
        }
    }
}
