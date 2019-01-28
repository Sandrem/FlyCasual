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
                    31,
                    extraUpgradeIcon: UpgradeType.Illicit,
                    factionOverride: Faction.Scum,
                    seImageNumber: 168
                );

                ModelInfo.SkinName = "Brown";
            }
        }
    }
}
