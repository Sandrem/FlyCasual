using System;
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
                PilotInfo = new PilotCardInfo(
                    "104th Battalion Pilot",
                    2,
                    42,
                    factionOverride: Faction.Republic
                );

                ModelInfo.SkinName = "Red";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/0d/16/0d16748c-6591-4e97-96ee-8db6c89abca5/swz33_battalion-pilot.png";
            }
        }
    }
}
