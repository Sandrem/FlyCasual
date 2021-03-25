using System;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLA4YWing
    {
        public class JinataSecurityOfficer : BTLA4YWing
        {
            public JinataSecurityOfficer() : base()
            {
                PilotInfo = new PilotCardInfo
                (
                    "Jinata Security Officer",
                    2,
                    31,
                    extraUpgradeIcon: UpgradeType.Tech,
                    factionOverride: Faction.Scum
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/e6/7f/e67f3145-67ad-4175-8a48-b92d87e58c28/swz85_pilot_jinatasecurityofficer.png";
            }
        }
    }
}
