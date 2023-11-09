using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESkStriker
    {
        public class BlackSquadronScout : TIESkStriker
        {
            public BlackSquadronScout() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Black Squadron Scout",
                    "",
                    Faction.Imperial,
                    3,
                    4,
                    5,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Gunner,
                        UpgradeType.Device
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 120
                );
            }
        }
    }
}
