using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class ZetaSquadronPilot : TIEFoFighter
        {
            public ZetaSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Zeta Squadron Pilot",
                    "",
                    Faction.FirstOrder,
                    2,
                    3,
                    3,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Tech,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/c0/af/c0afde49-7f44-4c59-8051-cc4140a04be0/swz26_a1_zeta-pilot.png";
            }
        }
    }
}
