using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AuzituckGunship
    {
        public class Wullffwarro : AuzituckGunship
        {
            public Wullffwarro() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Wullffwarro",
                    4,
                    56,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.WullffwarroAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 31
                );
            }
        }
    }
}
