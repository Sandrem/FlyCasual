using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESfFighter
    {
        public class ZetaSquadronSurvivor : TIESfFighter
        {
            public ZetaSquadronSurvivor() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Zeta Squadron Survivor",
                    "",
                    Faction.FirstOrder,
                    2,
                    4,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/848db1993150bda19217e2c14b3c3df6.png";
            }
        }
    }
}
