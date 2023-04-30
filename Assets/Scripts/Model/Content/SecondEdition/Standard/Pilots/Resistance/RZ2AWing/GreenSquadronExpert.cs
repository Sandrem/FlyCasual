using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ2AWing
    {
        public class GreenSquadronExpert : RZ2AWing
        {
            public GreenSquadronExpert() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Green Squadron Expert",
                    "",
                    Faction.Resistance,
                    3,
                    4,
                    2,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.AWing
                    },
                    skinName: "Green"
                );
            }
        }
    }
}