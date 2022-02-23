using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEDefender
    {
        public class MaarekStele : TIEDefender
        {
            public MaarekStele() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Maarek Stele",
                    7,
                    35,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.MaarekSteleAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

