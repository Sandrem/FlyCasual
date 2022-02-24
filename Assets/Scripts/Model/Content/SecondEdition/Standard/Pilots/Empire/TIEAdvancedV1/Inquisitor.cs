using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedV1
    {
        public class Inquisitor : TIEAdvancedV1
        {
            public Inquisitor() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Inquisitor",
                    "",
                    Faction.Imperial,
                    3,
                    4,
                    5,
                    force: 1,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.ForcePower
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 102
                );
            }
        }
    }
}