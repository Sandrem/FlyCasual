using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class BlueSquadronEscort : T65XWing
        {
            public BlueSquadronEscort() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Blue Squadron Escort",
                    "",
                    Faction.Rebel,
                    2,
                    5,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Astromech
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    },
                    seImageNumber: 11,
                    skinName: "Blue"
                );
            }
        }
    }
}
