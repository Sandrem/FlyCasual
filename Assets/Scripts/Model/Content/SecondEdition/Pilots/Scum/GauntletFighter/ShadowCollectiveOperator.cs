using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class ShadowCollectiveOperator : GauntletFighter
        {
            public ShadowCollectiveOperator() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Shadow Collective Operator",
                    "Stoic Super Commando",
                    Faction.Scum,
                    1,
                    7,
                    10,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian 
                    },
                    skinName: "Red"
                );

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/8/8f/Shadowcollectiveoperator.png";
            }
        }
    }
}