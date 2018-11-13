using System.Collections;
using System.Collections.Generic;
using Ship;
using SubPhases;
using Tokens;
using Abilities.FirstEdition;

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
                    limited: 1,
                    abilityType: typeof(GarvenDreisAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 4;
            }
        }
    }
}
