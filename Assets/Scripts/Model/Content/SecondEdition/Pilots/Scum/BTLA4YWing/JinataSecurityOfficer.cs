using Content;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Jinata Security Officer",
                    "",
                    Faction.Scum,
                    2,
                    4,
                    5,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Turret,
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Device
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/e6/7f/e67f3145-67ad-4175-8a48-b92d87e58c28/swz85_pilot_jinatasecurityofficer.png";
            }
        }
    }
}
