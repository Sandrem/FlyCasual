using Content;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.ModifiedTIELnFighter
    {
        public class MiningGuildSentry : ModifiedTIELnFighter
        {
            public MiningGuildSentry() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Mining Guild Sentry",
                    "",
                    Faction.Scum,
                    1,
                    3,
                    0,
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/4/44/Swz23_mining-guild-sentry.png";
            }
        }
    }
}
