using Content;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.TIEReaper
    {
        public class ScarifBasePilot : TIEReaper
        {
            public ScarifBasePilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Scarif Base Pilot",
                    "",
                    Faction.Imperial,
                    1,
                    4,
                    6,
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 116
                );
            }
        }
    }
}
