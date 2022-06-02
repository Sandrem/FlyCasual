using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Fireball
    {
        public class ColossusStationMechanic : Fireball
        {
            public ColossusStationMechanic() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Colossus Station Mechanic",
                    "",
                    Faction.Resistance,
                    2,
                    3,
                    5,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    }
                );

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/533ab83e881838eb8006c8f8dcf19145.png";
            }
        }
    }
}