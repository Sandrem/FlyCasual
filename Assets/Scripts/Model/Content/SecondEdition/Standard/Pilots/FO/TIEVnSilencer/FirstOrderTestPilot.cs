using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEVnSilencer
    {
        public class FirstOrderTestPilot : TIEVnSilencer
        {
            public FirstOrderTestPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "First Order Test Pilot",
                    "",
                    Faction.FirstOrder,
                    4,
                    5,
                    5,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Torpedo
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );
            }
        }
    }
}
