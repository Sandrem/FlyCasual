using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESkStriker
    {
        public class PlanetarySentinel : TIESkStriker
        {
            public PlanetarySentinel() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Planetary Sentinel",
                    "",
                    Faction.Imperial,
                    1,
                    4,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 121
                );
            }
        }
    }
}
