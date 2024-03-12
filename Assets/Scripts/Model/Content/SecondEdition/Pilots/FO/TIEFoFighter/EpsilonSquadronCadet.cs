using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEFoFighter
    {
        public class EpsilonSquadronCadet : TIEFoFighter
        {
            public EpsilonSquadronCadet() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Epsilon Squadron Cadet",
                    "",
                    Faction.FirstOrder,
                    1,
                    3,
                    2,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Tech
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/1/18/Swz26_a1_epsilon-pilot.png";
            }
        }
    }
}
