using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class BountyHunter : FiresprayClassPatrolCraft
        {
            public BountyHunter() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Bounty Hunter",
                    "",
                    Faction.Scum,
                    2,
                    7,
                    10,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Illicit
                    },
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter
                    },
                    seImageNumber: 154,
                    skinName: "Mandalorian Mercenary"
                );
            }
        }
    }
}
