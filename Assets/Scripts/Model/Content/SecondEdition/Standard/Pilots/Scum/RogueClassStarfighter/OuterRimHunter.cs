using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RogueClassStarfighter
    {
        public class OuterRimHunter : RogueClassStarfighter
        {
            public OuterRimHunter() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Outer Rim Hunter",
                    "",
                    Faction.Scum,
                    3,
                    5,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Illicit,
                        UpgradeType.Modification
                    }
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/outerrimhunter.png";
            }
        }
    }
}