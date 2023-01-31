using Content;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.YT2400LightFreighter
    {
        public class WildSpaceFringer : YT2400LightFreighter
        {
            public WildSpaceFringer() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Wild Space Fringer",
                    "",
                    Faction.Rebel,
                    1,
                    8,
                    7,
                    tags: new List<Tags>
                    {
                        Tags.Freighter
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Missile,
                        UpgradeType.Illicit,
                    },
                    seImageNumber: 79,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}
