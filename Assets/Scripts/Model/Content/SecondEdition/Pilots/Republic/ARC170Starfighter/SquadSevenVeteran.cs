using Content;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Squad Seven Veteran",
                    "",
                    Faction.Republic,
                    3,
                    5,
                    10,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Astromech,
                        UpgradeType.Gunner,
                        UpgradeType.Gunner,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    },
                    skinName: "Red"
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/a2/4e/a24eeedb-2b56-427b-90a0-142230928a02/swz33_sqd-7-vet.png";
            }
        }
    }
}
