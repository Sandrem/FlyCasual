using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class ImperialSuperCommando : GauntletFighter
        {
            public ImperialSuperCommando() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Imperial Super Commando",
                    "",
                    Faction.Imperial,
                    2,
                    7,
                    10,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian 
                    },
                    skinName: "Gray"
                );

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/c/c9/Imperialsupercommando.png";
            }
        }
    }
}