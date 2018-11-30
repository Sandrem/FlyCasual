using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.YT2400
    {
        public class DashRendar : YT2400
        {
            public DashRendar() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Dash Rendar",
                    7,
                    36,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.DashRendarAbility),
                    extraUpgradeIcon: UpgradeType.Elite
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class DashRendarAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnActivationPhaseStart += ActivateDashRendarAbility;
            Phases.Events.OnCombatPhaseStart_NoTriggers += DeactivateDashRendarAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnActivationPhaseStart -= ActivateDashRendarAbility;
            Phases.Events.OnCombatPhaseStart_NoTriggers -= DeactivateDashRendarAbility;
        }

        private void ActivateDashRendarAbility()
        {
            HostShip.IsIgnoreObstacles = true;
        }

        private void DeactivateDashRendarAbility()
        {
            HostShip.IsIgnoreObstacles = false;
        }

    }
}
