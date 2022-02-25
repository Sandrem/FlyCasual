using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ASF01BWing
    {
        public class BladeSquadronVeteran : ASF01BWing
        {
            public BladeSquadronVeteran() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Blade Squadron Veteran",
                    "",
                    Faction.Rebel,
                    3,
                    5,
                    6,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Torpedo
                    },
                    tags: new List<Tags>
                    {
                        Tags.BWing
                    },
                    seImageNumber: 25,
                    skinName: "Blue"
                );
            }
        }
    }
}
