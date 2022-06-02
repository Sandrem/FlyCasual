using System.Collections.Generic;
using Upgrade;
using Content;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedV1
    {
        public class BaronOfTheEmpire : TIEAdvancedV1
        {
            public BaronOfTheEmpire() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Baron of the Empire",
                    "",
                    Faction.Imperial,
                    3,
                    4,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 101
                );
            }
        }
    }
}