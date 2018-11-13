using System.Collections;
using System.Collections.Generic;
using Abilities.FirstEdition;

namespace Ship
{
    namespace FirstEdition.AWing
    {
        public class TychoCelchu : AWing
        {
            public TychoCelchu() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Tycho Celchu",
                    8,
                    26,
                    limited: 1,
                    abilityType: typeof(TychoCelchuAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class TychoCelchuAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.CanPerformActionsWhileStressed = true;
        }

        public override void DeactivateAbility()
        {
            HostShip.CanPerformActionsWhileStressed = false;
        }
    }
}