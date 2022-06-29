using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class NiteOwlLiberator : GauntletFighter
        {
            public NiteOwlLiberator() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Nite Owl Liberator",
                    "Resolute Warrior",
                    Faction.Republic,
                    2,
                    7,
                    16,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian
                    },
                    skinName: "Blue"
                );

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/7/76/Niteowlliberator.png";
            }
        }
    }
}