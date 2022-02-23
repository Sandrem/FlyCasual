using Content;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.ModifiedYT1300LightFreighter
    {
        public class OuterRimSmuggler : ModifiedYT1300LightFreighter
        {
            public OuterRimSmuggler() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Outer Rim Smuggler",
                    "",
                    Faction.Rebel,
                    1,
                    7,
                    6,
                    tags: new List<Tags>
                    {
                        Tags.Freighter,
                        Tags.YT1300
                    },
                    seImageNumber: 72
                );
            }
        }
    }
}
