using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ST70AssaultShip
    {
        public class OuterRimEnforcer : ST70AssaultShip
        {
            public OuterRimEnforcer() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Outer Rim Enforcer",
                    "",
                    Faction.Scum,
                    2,
                    6,
                    10,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    skinName: "Red Stripes"
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/outerrimenforcer.png";
            }
        }
    }
}