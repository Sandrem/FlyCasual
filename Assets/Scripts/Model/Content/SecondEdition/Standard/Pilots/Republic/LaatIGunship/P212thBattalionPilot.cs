using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.LaatIGunship
    {
        public class P212thBattalionPilot : LaatIGunship
        {
            public P212thBattalionPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "212th Battalion Pilot",
                    "",
                    Faction.Republic,
                    2,
                    5,
                    7,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Missile
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/80/e7/80e7698b-13df-4d66-ba2b-575df467a7df/swz70_a1_battalion-pilot_ship.png";
            }
        }
    }
}