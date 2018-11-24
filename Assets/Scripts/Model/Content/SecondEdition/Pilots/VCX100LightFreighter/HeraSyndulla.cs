﻿using Upgrade;

namespace Ship
{
    namespace SecondEdition.VCX100LightFreighter
    {
        public class HeraSyndulla : VCX100LightFreighter
        {
            public HeraSyndulla() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hera Syndulla",
                    5,
                    76,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.HeraSyndullaAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 73
                );
            }
        }
    }
}