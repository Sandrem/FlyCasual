using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ARC170Starfighter
    {
        public class P104thBattalionPilot : ARC170Starfighter
        {
            public P104thBattalionPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "104th Battalion Pilot",
                    "",
                    Faction.Republic,
                    2,
                    5,
                    8,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Torpedo,
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

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/0d/16/0d16748c-6591-4e97-96ee-8db6c89abca5/swz33_battalion-pilot.png";
            }
        }
    }
}
