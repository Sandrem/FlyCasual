using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.StarViperClassAttackPlatform
    {
        public class BlackSunEnforcer : StarViperClassAttackPlatform
        {
            public BlackSunEnforcer() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Black Sun Enforcer",
                    "",
                    Faction.Scum,
                    2,
                    5,
                    6,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Tech
                    },
                    seImageNumber: 182,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}