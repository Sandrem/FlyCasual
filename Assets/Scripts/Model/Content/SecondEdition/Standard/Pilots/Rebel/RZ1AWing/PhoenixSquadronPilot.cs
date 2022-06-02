using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class PhoenixSquadronPilot : RZ1AWing
        {
            public PhoenixSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Phoenix Squadron Pilot",
                    "",
                    Faction.Rebel,
                    1,
                    4,
                    3,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.AWing
                    },
                    seImageNumber: 22,
                    skinName: "Phoenix Squadron"
                );
            }
        }
    }
}