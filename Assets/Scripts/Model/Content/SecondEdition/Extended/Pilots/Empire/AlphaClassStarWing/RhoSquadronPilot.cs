using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AlphaClassStarWing
    {
        public class RhoSquadronPilot : AlphaClassStarWing
        {
            public RhoSquadronPilot() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Rho Squadron Pilot",
                    "",
                    Faction.Imperial,
                    3,
                    5,
                    9,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    },
                    seImageNumber: 137,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}
