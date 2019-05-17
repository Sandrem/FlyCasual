using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ARC170Starfighter
    {
        public class SquadSevenVeteran : ARC170Starfighter
        {
            public SquadSevenVeteran() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Squad Seven Veteran",
                    3,
                    47,
                    factionOverride: Faction.Republic,
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ModelInfo.SkinName = "Red";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/a2/4e/a24eeedb-2b56-427b-90a0-142230928a02/swz33_sqd-7-vet.png";
            }
        }
    }
}
