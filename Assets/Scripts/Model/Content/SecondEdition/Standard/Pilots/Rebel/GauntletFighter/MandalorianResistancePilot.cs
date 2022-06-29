using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class MandalorianResistancePilot : GauntletFighter
        {
            public MandalorianResistancePilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Mandalorian Resistance Pilot",
                    "Clan Loyalist",
                    Faction.Rebel,
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
                    skinName: "Blue"
                );

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/a/a6/Mandalorianresistancepilot.png";
            }
        }
    }
}