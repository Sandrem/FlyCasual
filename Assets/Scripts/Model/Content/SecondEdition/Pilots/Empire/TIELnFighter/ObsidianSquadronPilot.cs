using Content;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class ObsidianSquadronPilot : TIELnFighter
        {
            public ObsidianSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Obsidian Squadron Pilot",
                    "",
                    Faction.Imperial,
                    2,
                    2,
                    0,
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 91
                );
            }
        }
    }
}
