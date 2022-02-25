using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class OmegaSquadronAce : TIEFoFighter
        {
            public OmegaSquadronAce() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Omega Squadron Ace",
                    "",
                    Faction.FirstOrder,
                    3,
                    3,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/5f/de/5fde2469-451e-46eb-ad4e-936ff1d86935/swz26_a1_omega-ace.png";
            }
        }
    }
}
