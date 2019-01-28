using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class HiredGun : BTLA4YWing
        {
            public HiredGun() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hired Gun",
                    2,
                    33,
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Illicit },
                    factionOverride: Faction.Scum,
                    seImageNumber: 167
                );

                ModelInfo.SkinName = "Gray";
            }
        }
    }
}
