using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Hwk290LightFreighter
    {
        public class SpiceRunner : Hwk290LightFreighter
        {
            public SpiceRunner() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Spice Runner",
                    "",
                    Faction.Scum,
                    1,
                    3,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Device,
                        UpgradeType.Illicit
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter
                    },
                    seImageNumber: 177
                );
            }
        }
    }
}
