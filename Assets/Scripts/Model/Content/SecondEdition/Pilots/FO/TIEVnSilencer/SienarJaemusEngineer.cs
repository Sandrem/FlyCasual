using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEVnSilencer
    {
        public class SienarJaemusEngineer : TIEVnSilencer
        {
            public SienarJaemusEngineer() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Sienar-Jaemus Engineer",
                    "",
                    Faction.FirstOrder,
                    1,
                    5,
                    5,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Tech,
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
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
