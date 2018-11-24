using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AuzituckGunship
    {
        public class Lowhhrick : AuzituckGunship
        {
            public Lowhhrick() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lowhhrick",
                    3,
                    52,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.LowhhrickAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 32
                );

                ModelInfo.SkinName = "Lowhhrick";
            }
        }
    }
}