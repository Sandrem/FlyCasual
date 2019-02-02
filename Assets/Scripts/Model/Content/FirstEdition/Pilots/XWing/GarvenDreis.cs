using System.Collections;
using System.Collections.Generic;
using Ship;
using SubPhases;
using Tokens;
using Abilities.FirstEdition;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.XWing
    {
        public class GarvenDreis : XWing
        {
            public GarvenDreis() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Garven Dreis",
                    6,
                    26,
                    isLimited: true,
                    abilityType: typeof(GarvenDreisAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}
