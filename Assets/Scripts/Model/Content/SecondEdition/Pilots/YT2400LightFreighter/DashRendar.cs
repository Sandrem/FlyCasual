using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.YT2400LightFreighter
    {
        public class DashRendar : YT2400LightFreighter
        {
            public DashRendar() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Dash Rendar",
                    5,
                    100,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DashRendarAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 77
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DashRendarAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.IsIgnoreObstacles = true;
        }

        public override void DeactivateAbility()
        {
            HostShip.IsIgnoreObstacles = false;
        }
    }
}
