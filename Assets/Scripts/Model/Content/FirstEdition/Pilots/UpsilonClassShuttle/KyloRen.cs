using Conditions;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.UpsilonClassShuttle
    {
        public class KyloRen : UpsilonClassShuttle
        {
            public KyloRen() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kylo Ren",
                    6,
                    34,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.KyloRenPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}


