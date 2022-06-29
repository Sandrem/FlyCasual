using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class DeathWatchWarrior : GauntletFighter
        {
            public DeathWatchWarrior() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Death Watch Warrior",
                    "Fanatical Adheren",
                    Faction.Separatists,
                    2,
                    7,
                    10,
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
                    skinName: "CIS Dark"
                );

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/e/ed/Deathwatchwarrior.png";
            }
        }
    }
}