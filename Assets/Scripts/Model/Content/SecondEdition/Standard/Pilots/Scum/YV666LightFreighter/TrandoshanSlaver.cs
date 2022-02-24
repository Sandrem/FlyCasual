using Content;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.YV666LightFreighter
    {
        public class TrandoshanSlaver : YV666LightFreighter
        {
            public TrandoshanSlaver() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Trandoshan Slaver",
                    "",
                    Faction.Scum,
                    2,
                    6,
                    6,
                    tags: new List<Tags>
                    {
                        Tags.Freighter
                    },
                    seImageNumber: 213
                );
            }
        }
    }
}