using Content;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ1AWing
    {
        public class GreenSquadronPilot : RZ1AWing
        {
            public GreenSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Green Squadron Pilot",
                    "",
                    Faction.Rebel,
                    3,
                    4,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.AWing
                    },
                    seImageNumber: 21,
                    skinName: "Green"
                );
            }
        }
    }
}