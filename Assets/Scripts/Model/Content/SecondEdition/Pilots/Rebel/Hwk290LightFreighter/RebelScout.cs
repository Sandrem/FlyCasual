using Content;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Hwk290LightFreighter
    {
        public class RebelScout : Hwk290LightFreighter
        {
            public RebelScout() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Rebel Scout",
                    "",
                    Faction.Rebel,
                    2,
                    4,
                    6,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter
                    },
                    seImageNumber: 45
                );
            }
        }
    }
}
