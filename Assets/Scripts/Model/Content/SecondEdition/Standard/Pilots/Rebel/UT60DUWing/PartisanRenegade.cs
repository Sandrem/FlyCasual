using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.UT60DUWing
    {
        public class PartisanRenegade : UT60DUWing
        {
            public PartisanRenegade() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Partisan Renegade",
                    "",
                    Faction.Rebel,
                    1,
                    5,
                    6,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Illicit
                    },
                    tags: new List<Tags>
                    {
                        Tags.Partisan
                    },
                    seImageNumber: 61,
                    skinName: "Partisan"
                );
            }
        }
    }
}
