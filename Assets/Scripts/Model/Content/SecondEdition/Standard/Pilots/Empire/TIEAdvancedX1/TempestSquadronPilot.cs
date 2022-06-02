using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedX1
    {
        public class TempestSquadronPilot : TIEAdvancedX1
        {
            public TempestSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Tempest Squadron Pilot",
                    "",
                    Faction.Imperial,
                    2,
                    4,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {

                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 98
                );
            }
        }
    }
}