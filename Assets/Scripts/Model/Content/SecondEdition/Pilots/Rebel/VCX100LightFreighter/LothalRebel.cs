using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.VCX100LightFreighter
    {
        public class LothalRebel : VCX100LightFreighter
        {
            public LothalRebel() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Lothal Rebel",
                    "",
                    Faction.Rebel,
                    2,
                    7,
                    8,
                    tags: new List<Tags>
                    {
                        Tags.Freighter
                    },
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Turret,
                        UpgradeType.Torpedo,
                        UpgradeType.Gunner
                    },
                    seImageNumber: 76
                );
            }
        }
    }
}
