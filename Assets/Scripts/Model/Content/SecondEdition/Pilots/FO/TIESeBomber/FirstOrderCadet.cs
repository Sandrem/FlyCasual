using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESeBomber
    {
        public class FirstOrderCadet : TIESeBomber
        {
            public FirstOrderCadet() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "First Order Cadet",
                    "",
                    Faction.FirstOrder,
                    3,
                    4,
                    7,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Tech,
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://i.imgur.com/VWm0SbB.png";
            }
        }
    }
}
