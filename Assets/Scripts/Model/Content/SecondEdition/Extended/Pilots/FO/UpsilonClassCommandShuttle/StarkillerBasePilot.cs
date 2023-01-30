using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.UpsilonClassCommandShuttle
    {
        public class StarkillerBasePilot : UpsilonClassCommandShuttle
        {
            public StarkillerBasePilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Starkiller Base Pilot",
                    "",
                    Faction.FirstOrder,
                    2,
                    7,
                    8,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Tech,
                        UpgradeType.Tech,
                        UpgradeType.Cannon,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Modification
                    },
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/41f6d936f14a058ed1c5e6ac12de37c2.png";
            }
        }
    }
}
