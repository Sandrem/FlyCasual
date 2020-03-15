using System;
using System.Linq;
using BoardTools;
using Ship;
using SubPhases;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Fireball
    {
        public class ColossusStationMechanic : Fireball
        {
            public ColossusStationMechanic() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Colossus Station Mechanic",
                    2,
                    26,
                    extraUpgradeIcon: UpgradeType.Astromech
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/533ab83e881838eb8006c8f8dcf19145.png";
            }
        }
    }
}